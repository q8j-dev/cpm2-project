using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Errors;
using JetBrains.Annotations;
using Unions;
using UnityEngine;

namespace Firebase.Impl
{
	public class Auth : FirebaseAuth
	{
		private class MetadataImpl : Metadata
		{
			public override ulong CreationTimestamp { get; }

			public MetadataImpl(ulong creationTimestamp)
			{
				CreationTimestamp = creationTimestamp;
			}
		}

		private class User : FirebaseUser
		{
			private const int ConfirmEmailRequestType = 4;

			private string Password { get; }

			public override string UserId { get; }

			public override string DisplayName { get; }

			public override bool IsEmailVerified { get; }

			public override string Email { get; }

			public override bool IsAnonymous { get; }

			public override Metadata Metadata { get; }

			public User(string displayName, string password, UserRes accInfoUser)
			{
				Password = password;
				UserId = accInfoUser.localId;
				DisplayName = displayName;
				Email = accInfoUser.email;
				IsEmailVerified = accInfoUser.emailVerified;
				Metadata = new MetadataImpl(ulong.Parse(accInfoUser.createdAt));
			}

			public override async UniTask<bool> SendEmailVerificationAsync()
			{
				return await (await GetSignInRes(Email, Password)).Match(OnSuccess, (NoInternetConnection _) => UniTask.FromResult(value: false), (HttpError _) => UniTask.FromResult(value: false), (GoogleError _) => UniTask.FromResult(value: false));
				static async UniTask<bool> OnSuccess(SignInRes res)
				{
					return (await Utils.PostAsyncWithErrorHandling<ConfirmEmailArg, ConfirmEmailRes>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/getOobConfirmationCode?key=AIzaSyCQDz9rgjgmvmFkvVfmvr2-7fT4tfrzRRQ", new ConfirmEmailArg(4, res.idToken))).Match((ConfirmEmailRes _) => true, (NoInternetConnection _) => false, (HttpError _) => false, (GoogleError _) => false);
				}
			}

			public override UniTask<Union<FirebaseUser, CreationError>> LinkWithCredential(Credential credential)
			{
				throw new NotImplementedException();
			}
		}

		[Serializable]
		[UsedImplicitly]
		private class ResetPasswordArg
		{
			public int requestType;

			public string email;

			public bool androidInstallApp;

			public bool canHandleCodeInApp;

			public string clientType;

			public ResetPasswordArg(int requestType, string email)
			{
				this.requestType = requestType;
				this.email = email;
				clientType = "CLIENT_TYPE_ANDROID";
				androidInstallApp = false;
				canHandleCodeInApp = false;
			}
		}

		[Serializable]
		private class ResetPasswordRes
		{
			public string kind;

			public string email;
		}

		[Serializable]
		[UsedImplicitly]
		private class GetAccountInfoArg
		{
			public string idToken;

			public GetAccountInfoArg(string idToken)
			{
				this.idToken = idToken;
			}
		}

		[Serializable]
		private class GetAccountInfoRes
		{
			public string kind;

			public List<UserRes> users;
		}

		[Serializable]
		private class UserRes
		{
			public string localId;

			public string email;

			public string passwordHash;

			public bool emailVerified;

			public long passwordUpdatedAt;

			public List<ProviderUserInfo> providerUserInfo;

			public string validSince;

			public string lastLoginAt;

			public string createdAt;

			public string lastRefreshAt;
		}

		[Serializable]
		private class ProviderUserInfo
		{
			public string providerId;

			public string federatedId;

			public string email;

			public string rawId;
		}

		[Serializable]
		[UsedImplicitly]
		private class SignInArg
		{
			public string email;

			public string password;

			public bool returnSecureToken;

			public string clientType;

			public SignInArg(string email, string password)
			{
				this.email = email;
				this.password = password;
				returnSecureToken = true;
				clientType = "CLIENT_TYPE_ANDROID";
			}
		}

		[Serializable]
		[UsedImplicitly]
		private class SignUpArg
		{
			public string email;

			public string password;

			public string clientType;

			public SignUpArg(string email, string password)
			{
				this.email = email;
				this.password = password;
				clientType = "CLIENT_TYPE_ANDROID";
			}
		}

		[Serializable]
		private class SignUpRes
		{
			public string kind;

			public string localId;

			public string email;

			public string idToken;

			public string refreshToken;

			public string expiresIn;
		}

		[Serializable]
		private class SignInRes
		{
			public string kind;

			public string localId;

			public string email;

			public string displayName;

			public string idToken;

			public bool registered;

			public string refreshToken;

			public string expiresIn;
		}

		[Serializable]
		private class CustomTokenArg
		{
			public string token;

			public bool returnSecureToken;

			public CustomTokenArg(string token)
			{
				this.token = token;
				returnSecureToken = true;
			}
		}

		[Serializable]
		private class CustomTokenRes
		{
			public string idToken;

			public string refreshToken;

			public string expiresIn;
		}

		[Serializable]
		[UsedImplicitly]
		private class ConfirmEmailArg
		{
			public int requestType;

			public string idToken;

			public string clientType;

			public ConfirmEmailArg(int requestType, string idToken)
			{
				this.requestType = requestType;
				this.idToken = idToken;
				clientType = "CLIENT_TYPE_ANDROID";
			}
		}

		[Serializable]
		private class ConfirmEmailRes
		{
			public string kind;

			public string email;
		}

		private const int ResetPasswordRequestType = 1;

		private FirebaseUser _currentUser;

		private const string ClientType = "CLIENT_TYPE_ANDROID";

		private const double RefreshThresholdSeconds = 120.0;

		private const double MinAutoRefreshPollSeconds = 15.0;

		private const double MaxAutoRefreshPollSeconds = 30.0;

		private readonly TokenRefreshSchedule _schedule = new TokenRefreshSchedule(120.0);

		private UniTaskCompletionSource _refreshInFlight;

		private bool _autoRefreshLoopStarted;

		private int _sessionEpoch;

		private const string KEY = "AIzaSyCQDz9rgjgmvmFkvVfmvr2-7fT4tfrzRRQ";

		private static readonly CreationError _unknown = new CreationError.Unknown();

		private static readonly CreationError _emailExists = new CreationError.EmailExists();

		private Union<FirebaseUser, SignInError> _lastResult;

		private DateTime? _lastResultTime;

		private int _emailHash;

		private int _passwordHash;

		private string _refreshToken;

		public override FirebaseUser CurrentUser => _currentUser;

		private int SessionEpoch => _sessionEpoch;

		private static double Now => (double)DateTime.UtcNow.Ticks / 10000000.0;

		public override async UniTask SendPasswordResetEmail(string email)
		{
			await Utils.PostAsyncWithErrorHandling<ResetPasswordArg, ResetPasswordRes>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/getOobConfirmationCode?key=AIzaSyCQDz9rgjgmvmFkvVfmvr2-7fT4tfrzRRQ", new ResetPasswordArg(1, email));
		}

		private void OnTokenIssued(string refreshToken, string expiresInSeconds)
		{
			_refreshToken = refreshToken;
			_schedule.OnTokenIssued(Now, expiresInSeconds);
			EnsureAutoRefreshLoop();
		}

		public override async UniTask RefreshToken(bool force = false)
		{
			if (_refreshToken == null)
			{
				Debug.LogWarning("No refresh token! User is not signed in");
				return;
			}
			if (force)
			{
				_schedule.ForceExpire();
			}
			if (!_schedule.ShouldRefresh(Now))
			{
				return;
			}
			if (_refreshInFlight == null)
			{
				int sessionEpoch = _sessionEpoch;
				_refreshInFlight = new UniTaskCompletionSource();
				try
				{
					(await Utils.PostAsyncWithErrorHandling<RefreshTokenArg, RefreshTokenRes>("https://securetoken.googleapis.com/v1/token?key=AIzaSyCQDz9rgjgmvmFkvVfmvr2-7fT4tfrzRRQ", new RefreshTokenArg(_refreshToken))).Match((RefreshTokenRes refreshTokenRes) =>
					{
						ApplyRefreshedToken(sessionEpoch, refreshTokenRes);
					}, ErrorUtils.Handler, ErrorUtils.Handler, ErrorUtils.Handler);
					return;
				}
				finally
				{
					_refreshInFlight.TrySetResult();
					_refreshInFlight = null;
				}
			}
			await _refreshInFlight.Task;
		}

		private void ApplyRefreshedToken(int sessionEpoch, RefreshTokenRes refreshTokenRes)
		{
			if (sessionEpoch == _sessionEpoch)
			{
				SetIdToken(refreshTokenRes.id_token);
				OnTokenIssued(refreshTokenRes.refresh_token, refreshTokenRes.expires_in);
			}
		}

		private void EnsureAutoRefreshLoop()
		{
			if (!_autoRefreshLoopStarted)
			{
				_autoRefreshLoopStarted = true;
				Application.focusChanged += OnApplicationFocusChanged;
				AutoRefreshLoop().Forget();
			}
		}

		private void OnApplicationFocusChanged(bool focused)
		{
			if (focused && _refreshToken != null && _schedule.ShouldRefresh(Now))
			{
				RefreshToken().Forget();
			}
		}

		private async UniTaskVoid AutoRefreshLoop()
		{
			while (true)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(Math.Clamp(_schedule.SecondsUntilRefresh(Now), 15.0, 30.0)), DelayType.Realtime);
				if (_refreshToken != null && _schedule.ShouldRefresh(Now))
				{
					try
					{
						await RefreshToken();
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
				}
			}
		}

		public override UniTask<Union<FirebaseUser, SignInError>> SignInAnonymously()
		{
			throw new NotImplementedException();
		}

		public override async UniTask<Union<FirebaseUser, CreationError>> CreateUserWithEmailAndPassword(string email, string password)
		{
			return await (await Utils.PostAsyncWithErrorHandling<SignUpArg, SignUpRes>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=AIzaSyCQDz9rgjgmvmFkvVfmvr2-7fT4tfrzRRQ", new SignUpArg(email, password))).Match((Func<SignUpRes, UniTask<Union<FirebaseUser, CreationError>>>)(async (SignUpRes signUpRes) => (await Utils.PostAsyncWithErrorHandling<GetAccountInfoArg, GetAccountInfoRes>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/getAccountInfo?key=AIzaSyCQDz9rgjgmvmFkvVfmvr2-7fT4tfrzRRQ", new GetAccountInfoArg(signUpRes.idToken))).Match((Func<GetAccountInfoRes, Union<FirebaseUser, CreationError>>)((GetAccountInfoRes getAccountInfoRes) =>
			{
				SetIdToken(signUpRes.idToken);
				OnTokenIssued(signUpRes.refreshToken, signUpRes.expiresIn);
				User user = new User(email, password, getAccountInfoRes.users[0]);
				_currentUser = user;
				return user;
			}), (Func<NoInternetConnection, Union<FirebaseUser, CreationError>>)((NoInternetConnection _) => _unknown), (Func<HttpError, Union<FirebaseUser, CreationError>>)((HttpError _) => _unknown), (Func<GoogleError, Union<FirebaseUser, CreationError>>)((GoogleError _) => _unknown))), (Func<NoInternetConnection, UniTask<Union<FirebaseUser, CreationError>>>)((NoInternetConnection _) => UniTask.FromResult((Union<FirebaseUser, CreationError>)_unknown)), (Func<HttpError, UniTask<Union<FirebaseUser, CreationError>>>)((HttpError _) => UniTask.FromResult((Union<FirebaseUser, CreationError>)_unknown)), (Func<GoogleError, UniTask<Union<FirebaseUser, CreationError>>>)((GoogleError err) => UniTask.FromResult((Union<FirebaseUser, CreationError>)((err.message == "EMAIL_EXISTS") ? _emailExists : _unknown))));
		}

		private static void SetIdToken(string idToken)
		{
			Functions.FunctionHeaders["Authorization"] = "Bearer " + idToken;
		}

		public override async UniTask<Union<FirebaseUser, SignInError>> SignInWithEmailAndPassword(string email, string password)
		{
			if (_lastResultTime.HasValue && DateTime.Now - _lastResultTime.Value < TimeSpan.FromMinutes(5.0) && email.GetHashCode() == _emailHash && password.GetHashCode() == _passwordHash)
			{
				return _lastResult;
			}
			return await (await GetSignInRes(email, password)).Match((Func<SignInRes, UniTask<Union<FirebaseUser, SignInError>>>)(async (SignInRes res) =>
			{
				OnTokenIssued(res.refreshToken, res.expiresIn);
				return (await GetGetAccountInfoRes(res)).Match((Func<GetAccountInfoRes, Union<FirebaseUser, SignInError>>)((GetAccountInfoRes accountInfo) =>
				{
					SetIdToken(res.idToken);
					User user = new User(res.displayName, password, accountInfo.users[0]);
					_currentUser = user;
					_lastResult = user;
					_lastResultTime = DateTime.Now;
					_emailHash = email.GetHashCode();
					_passwordHash = password.GetHashCode();
					return user;
				}), (Func<NoInternetConnection, Union<FirebaseUser, SignInError>>)((NoInternetConnection _) => err()), (Func<HttpError, Union<FirebaseUser, SignInError>>)((HttpError _) => err()), (Func<GoogleError, Union<FirebaseUser, SignInError>>)((GoogleError googleError) => err(googleError)));
			}), (Func<NoInternetConnection, UniTask<Union<FirebaseUser, SignInError>>>)((NoInternetConnection _) => UniTask.FromResult((Union<FirebaseUser, SignInError>)err())), (Func<HttpError, UniTask<Union<FirebaseUser, SignInError>>>)((HttpError _) => UniTask.FromResult((Union<FirebaseUser, SignInError>)err())), (Func<GoogleError, UniTask<Union<FirebaseUser, SignInError>>>)((GoogleError googleError) => UniTask.FromResult((Union<FirebaseUser, SignInError>)err(googleError))));
			SignInError err(GoogleError error = null)
			{
				if (error?.message != "EMAIL_NOT_FOUND" && error?.message != "INVALID_PASSWORD")
				{
					Reset();
				}
				return new SignInError(error);
			}
		}

		public override async UniTask<Union<FirebaseUser, SignInError>> SignInWithCustomToken(string customToken)
		{
			return await (await GetSignInResWithCustomToken(customToken)).Match((Func<CustomTokenRes, UniTask<Union<FirebaseUser, SignInError>>>)(async (CustomTokenRes res) =>
			{
				OnTokenIssued(res.refreshToken, res.expiresIn);
				return (await GetGetAccountInfoRes(res)).Match((Func<GetAccountInfoRes, Union<FirebaseUser, SignInError>>)((GetAccountInfoRes accountInfo) =>
				{
					SetIdToken(res.idToken);
					User user = new User("", "", accountInfo.users[0]);
					_currentUser = user;
					_lastResult = user;
					_lastResultTime = DateTime.Now;
					_emailHash = 0;
					_passwordHash = 0;
					return user;
				}), (Func<NoInternetConnection, Union<FirebaseUser, SignInError>>)((NoInternetConnection _) => err()), (Func<HttpError, Union<FirebaseUser, SignInError>>)((HttpError _) => err()), (Func<GoogleError, Union<FirebaseUser, SignInError>>)((GoogleError googleError) => err(googleError)));
			}), (Func<NoInternetConnection, UniTask<Union<FirebaseUser, SignInError>>>)((NoInternetConnection _) => UniTask.FromResult((Union<FirebaseUser, SignInError>)err())), (Func<HttpError, UniTask<Union<FirebaseUser, SignInError>>>)((HttpError _) => UniTask.FromResult((Union<FirebaseUser, SignInError>)err())), (Func<GoogleError, UniTask<Union<FirebaseUser, SignInError>>>)((GoogleError googleError) => UniTask.FromResult((Union<FirebaseUser, SignInError>)err(googleError))));
			SignInError err(GoogleError error = null)
			{
				Reset();
				return new SignInError(error);
			}
		}

		private void Reset()
		{
			_sessionEpoch++;
			Functions.FunctionHeaders.Remove("Authorization");
			_currentUser = null;
			_lastResult = null;
			_lastResultTime = null;
			_refreshToken = null;
			_emailHash = 0;
			_passwordHash = 0;
			_schedule.Clear();
		}

		private static async UniTask<Union<GetAccountInfoRes, NoInternetConnection, HttpError, GoogleError>> GetGetAccountInfoRes(SignInRes res)
		{
			return await Utils.PostAsyncWithErrorHandling<GetAccountInfoArg, GetAccountInfoRes>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/getAccountInfo?key=AIzaSyCQDz9rgjgmvmFkvVfmvr2-7fT4tfrzRRQ", new GetAccountInfoArg(res.idToken));
		}

		private static async UniTask<Union<GetAccountInfoRes, NoInternetConnection, HttpError, GoogleError>> GetGetAccountInfoRes(CustomTokenRes res)
		{
			return await Utils.PostAsyncWithErrorHandling<GetAccountInfoArg, GetAccountInfoRes>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/getAccountInfo?key=AIzaSyCQDz9rgjgmvmFkvVfmvr2-7fT4tfrzRRQ", new GetAccountInfoArg(res.idToken));
		}

		private static async UniTask<Union<SignInRes, NoInternetConnection, HttpError, GoogleError>> GetSignInRes(string email, string password)
		{
			return await Utils.PostAsyncWithErrorHandling<SignInArg, SignInRes>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=AIzaSyCQDz9rgjgmvmFkvVfmvr2-7fT4tfrzRRQ", new SignInArg(email, password));
		}

		private static async UniTask<Union<CustomTokenRes, NoInternetConnection, HttpError, GoogleError>> GetSignInResWithCustomToken(string customToken)
		{
			return await Utils.PostAsyncWithErrorHandling<CustomTokenArg, CustomTokenRes>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyCustomToken?key=AIzaSyCQDz9rgjgmvmFkvVfmvr2-7fT4tfrzRRQ", new CustomTokenArg(customToken));
		}

		public override void SignOut()
		{
			Reset();
		}
	}
}
