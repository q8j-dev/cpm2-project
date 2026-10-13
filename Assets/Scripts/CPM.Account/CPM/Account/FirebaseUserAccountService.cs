using System;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Errors;
using Firebase.Functions;
using Newtonsoft.Json;
using UnityEngine;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.AccountSystemNew;

namespace CPM.Account
{
	public class FirebaseUserAccountService : IUserAccountService
	{
		public const string DeleteAccountFn = "DeleteAccount24_1";

		public const string IsEmailInUseFn = "IsEmailInUse24_1";

		public const string ChangeEmailAndPasswordFn = "ChangeEmailAndPassword24_1";

		private FirebaseAuth auth;

		private FirebaseWrappedFunctionFactory factory;

		private FirebaseAccountController _firebaseAccountController;

		public string LastSignInError { get; private set; }

		public FirebaseUserAccountService(FirebaseAccountController firebaseAccountController)
		{
			_firebaseAccountController = firebaseAccountController;
			factory = new FirebaseWrappedFunctionFactory();
		}

		public async UniTask<bool> CheckDependencies()
		{
			if (auth != null)
			{
				return true;
			}
			if (await FirebaseApp.CheckDependenciesAsync() == DependencyStatus.Available)
			{
				auth = FirebaseAuth.DefaultInstance;
			}
			return auth != null;
		}

		public async UniTask<bool> ResetPassword(string email)
		{
			UniTask uniTask = auth.SendPasswordResetEmail(email);
			try
			{
				await uniTask;
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				return false;
			}
			return true;
		}

		public async UniTask<SignUpData> SignUp(string email, string password)
		{
			SignUpResult signUpResult = SignUpResult.Unknown;
			FirebaseUser user = null;
			if (!(await CheckCanSignUp()))
			{
				return new SignUpData
				{
					firebaseUser = user,
					result = SignUpResult.ErrorCantSignUp
				};
			}
			if (auth.CurrentUser != null && auth.CurrentUser.IsAnonymous)
			{
				Debug.Log("ACCOUNT Create user link to anon");
				Credential credential = EmailAuthProvider.GetCredential(email, password);
				(await auth.CurrentUser.LinkWithCredential(credential)).Match((FirebaseUser fb) =>
				{
					signUpResult = SignUpResult.Success;
					user = fb;
					_firebaseAccountController.CreationTimestamp = user.Metadata.CreationTimestamp;
				}, (CreationError err) =>
				{
					if (!(err is CreationError.EmailExists))
					{
						if (err is CreationError.Unknown)
						{
							signUpResult = SignUpResult.ErrorUnknown;
							Debug.LogError("LinkWithCredentialAsync encountered an error: " + signUpResult);
						}
					}
					else
					{
						signUpResult = SignUpResult.ErrorUserExists;
					}
				});
				return new SignUpData
				{
					firebaseUser = user,
					result = signUpResult
				};
			}
			Debug.Log(" ACCOUNT Create user old way");
			(await auth.CreateUserWithEmailAndPassword(email, password)).Match((FirebaseUser fb) =>
			{
				signUpResult = SignUpResult.Success;
				user = fb;
				_firebaseAccountController.CreationTimestamp = user.Metadata.CreationTimestamp;
			}, (CreationError err) =>
			{
				if (!(err is CreationError.EmailExists))
				{
					if (err is CreationError.Unknown)
					{
						signUpResult = SignUpResult.ErrorUnknown;
						Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + signUpResult);
					}
				}
				else
				{
					signUpResult = SignUpResult.ErrorUserExists;
					Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + signUpResult);
				}
			});
			return new SignUpData
			{
				firebaseUser = user,
				result = signUpResult
			};
		}

		public async UniTask<SignUpData> ChangeEmailPassword(string emailText, string passwordText)
		{
			FirebaseWrappedFunction firebaseWrappedFunction = factory.Create("ChangeEmailAndPassword24_1");
			object value = new
			{
				email = emailText,
				password = passwordText
			};
			FunctionCallResult<EmptyClass> functionCallResult = await firebaseWrappedFunction.CallAsyncResult<EmptyClass>(JsonConvert.SerializeObject(value));
			SignUpData signUpData = new SignUpData
			{
				result = (functionCallResult.IsSuccess ? SignUpResult.Success : SignUpResult.Unknown)
			};
			if (signUpData.result == SignUpResult.Success)
			{
				FirebaseAuth.DefaultInstance.SignOut();
				await FirebaseAuth.DefaultInstance.SignInWithEmailAndPassword(emailText, passwordText);
			}
			signUpData.firebaseUser = auth.CurrentUser;
			return signUpData;
		}

		public async UniTask<FirebaseUser> SignIn(string email, string password, bool resetData)
		{
			await CheckDependencies();
			LastSignInError = null;
			return await (await auth.SignInWithEmailAndPassword(email, password)).Match((Func<FirebaseUser, UniTask<FirebaseUser>>)(async (FirebaseUser f) =>
			{
				if (resetData)
				{
					await _firebaseAccountController.ResetData(reloadScene: false);
				}
				_firebaseAccountController.CreationTimestamp = f.Metadata.CreationTimestamp;
				return f;
			}), (Func<SignInError, UniTask<FirebaseUser>>)((SignInError error) =>
			{
				LastSignInError = error?.Error?.message;
				return UniTask.FromResult<FirebaseUser>(null);
			}));
		}

		public void SignOut()
		{
			auth.SignOut();
		}

		public async UniTask DeleteAccount()
		{
			await factory.Create("DeleteAccount24_1").CallAsyncResult<object>(SystemInfo.deviceUniqueIdentifier);
		}

		public async UniTask<bool> HaveGuestAccount()
		{
			return await IsEmailInUse(AccountSystemV2Extension.GuestEmail);
		}

		private async UniTask<bool> IsEmailInUse(string email)
		{
			FunctionCallResult<int> functionCallResult = await factory.Create("IsEmailInUse24_1").CallAsyncResult<int>(email);
			return functionCallResult.IsSuccess && functionCallResult.Data == 1;
		}

		private async UniTask<bool> CheckCanSignUp()
		{
			return true;
		}
	}
}
