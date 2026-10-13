using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Scripting;

namespace CPM.Common
{
	[Preserve]
	public class ChatFilter : IChatFilterUtility
	{
		public static string[] wordsToRemove = new string[1050]
		{
			"anal", "ana1", "anus", "arse", "ass", "ballsack", "ba11sack", "balls", "ba11s", "bastard",
			"bitch", "b1tch", "biatch", "b1atch", "bloody", "b1oody", "blowjob", "b1owjob", "bollock", "bo11ock",
			"bollok", "bo11ok", "boner", "boob", "b00b", "bugger", "bum", "butt", "buttplug", "clitoris",
			"c1itoris", "cock", "c0ck", "coon", "c00n", "crap", "cunt", "damn", "dick", "d1ck",
			"dildo", "di1do", "dyke", "fag", "feck", "fellate", "fellatio", "felching", "fuck", "fack",
			"fudgepacker", "f1ange", "flange", "goddamn", "g0ddamn", "hell", "fuckyou", "he11", "homo", "h0m0",
			"jerk", "jizz", "j1zz", "knobend", "kn0bend", "labia", "lmao", "lmfao", "muff", "nigger",
			"nigga", "n1gger", "n1gga", "omg", "penis", "piss", "poop", "pen1s", "p1ss", "p00p",
			"prick", "pube", "pussy", "queer", "scrotum", "sex", "shit", "sh1t", "slut", "suck",
			"smegma", "spunk", "tit", "t1t", "tosser", "turd", "twat", "vagina", "vag1na", "wank",
			"whore", "wtf", "анус", "блять", "блядь", "блят", "бляд", "бля", "бла", "балда",
			"блядство", "вагина", "гандон", "говно", "гондон", "гавно", "гавна", "гей", "геи", "гомик",
			"гомо", "говнюк", "гавнюк", "говноед", "дурак", "дрочить", "долбаеб", "долбоеб", "далбаеб", "дибил",
			"дибилка", "жопа", "жополиз", "жопализ", "ебан", "ебал", "ебала", "еблан", "ебадь", "ебат",
			"ебад", "ебать", "ебаться", "ебанутый", "ебануть", "ебля", "ебанат", "ибан", "ибал", "иблан",
			"лесбиянка", "лесби", "мудак", "минет", "мать", "мама", "мразь", "манда", "нах", "наебать",
			"обосраться", "обосратся", "абасратся", "обасраться", "охуеть", "ахуеть", "ахуел", "охуел", "охует", "ахует",
			"проститутка", "педик", "пиздец", "пидараз", "пидаразы", "пидарас", "пидарасы", "придурок", "пизда", "пидр",
			"пох", "петух", "пиздобол", "котак", "пиздабол", "пиздить", "пиздит", "пиздун", "пиздюк", "срать",
			"ссать", "сосать", "сасать", "сасат", "сперма", "срат", "сука", "сопляк", "секс", "срака",
			"срун", "срач", "тупой", "урод", "уебок", "ублюдок", "хер", "хуй", "хуи", "хуесос",
			"хуйня", "хуиня", "хули", "хуя", "хуяк", "член", "черт", "шлюха", "сучка", "заебись",
			"драчить", "кончить", "кончать", "миньет", "дрочить", "дебил", "ебу", "ибу", "ебал", "ебля",
			"куни", "abazan", "amcık", "alçak", "amına", "amc1k", "a1çak", "am1na", "sikerim", "s1ker1m",
			"ananı", "anan1", "arza", "aptal", "apta1", "bakir", "bakire", "bak1r", "bak1re", "bok",
			"cadı", "b0k", "cad1", "cahil", "çatlak", "çirkin", "çiş", "cah1l", "çat1ak", "ç1rk1n",
			"ç1ş", "çük", "deli", "domuz", "eşşek", "eccek", "gebertirim", "gerizekalı", "gerizekal1", "götüne",
			"g0tüne", "hayvan", "ibne", "siktin", "s1kt1n", "kahpe", "kahpenin", "kaltak", "köpek", "kıro",
			"k1ro", "manyak", "orospu", "çocuğu", "çekmek", "piç", "salak", "sik", "s1k", "sikmek",
			"s1kmek", "sikişmek", "siktir", "sıçmak", "sıçtık", "s1k1şmek", "s1kt1r", "s1çmak", "s1çt1k", "şerefsiz",
			"taşak", "tacak", "tohum", "yarrak", "yavşak", "alcak", "catlak", "cirkin", "cis", "gotune",
			"cocugu", "cekmek", "pic", "kopek", "sıcmak", "sıctık", "serefsiz", "tasak", "yavsak", "↓↓↓",
			"死ね", "しねしね", "殺す", "ちんこ", "チンチン", "チンコ", "おちんこ", "オチンコ", "まんこ", "マンコ",
			"キチガイ", "ガイジ", "せっくす", "セックス", "オナニー", "クソ", "script", "gg", "gameguardian", "guardian",
			"vipscript", "vip", "hack", "geimgardian", "termux", "ipa", "mod", "modded", "apk", "cheat",
			"a.q", "a.q.", "abaza", "abazan", "ag", "ağzına sıçayım", "ahmak", "allahsız", "am biti", "amarım",
			"ambiti", "amcığı", "amcığın", "amcığını", "amcığınızı", "amcık", "amcık hoşafı", "amcıklama", "amcıklandı", "amcik",
			"amck", "amckl", "amcklama", "amcklaryla", "amckta", "amcktan", "amcuk", "amık", "amın feryadı", "amın oglu",
			"amın oğlu", "amına", "amına koy", "amına koyarım", "amına koyayım", "amına koyyim", "amına s", "amına sikem", "amına sokam", "amınako",
			"amınakoyim", "amını", "amını s", "amınoğlu", "amısına", "amısını", "amin oglu", "amina", "amina g", "amina k",
			"amina koyarim", "amina koyayım", "amina koyayim", "aminako", "aminakoyarim", "aminakoyim", "aminda", "amindan", "amindayken", "amini",
			"aminiyarraaniskiim", "aminoglu", "amiyum", "amk", "amk çocuğu", "amkafa", "amlarnzn", "amlı", "amm", "ammak",
			"ammna", "amn", "amna", "amnda", "amndaki", "amngtn", "amnn", "amona", "amq", "amsız",
			"amsiz", "amsz", "amteri", "amugaa", "amuğa", "amuna", "anaaann", "anal", "analarn", "anan",
			"anana", "anandan", "ananı", "ananı sikerim", "ananı sikeyim", "ananın", "ananın am", "ananın amı", "ananın dölü", "ananınki",
			"ananısikerim", "ananısikeyim", "ananızın", "ananızın am", "anani", "anani sikerim", "anani sikeyim", "ananin", "ananisikerim", "ananisikeyim",
			"anann", "ananz", "anas", "anası orospu", "anasını", "anasının am", "anasi", "anasinin", "anay", "anayin",
			"andaval", "angut", "annen", "anneni", "annenin", "annesiz", "anuna", "aptal", "aq", "aq.",
			"atkafası", "atmık", "attırdığım", "attrrm", "auzlu", "avrat", "aw", "awq", "ayklarmalrmsikerim", "azdım",
			"azdır", "azdırıcı", "babaannesi kaşar", "babanı", "babanın", "babani", "babası pezevenk", "bacağına sıçayım", "bacına", "bacını",
			"bacının", "bacini", "bacn", "bacndan", "bacy", "bastard", "basur", "beyinsiz", "bızır", "bitch",
			"biting", "bok", "boka", "bokbok", "bokça", "bokhu", "bokkkumu", "boklar", "boktan", "boku",
			"bokubokuna", "bokum", "bombok", "boner", "bosalmak", "boşalmak", "cenabet", "cibiliyetsiz", "cibilliyetini", "cibilliyetsiz",
			"cif", "cim", "çük", "dalaksız", "dallama", "daltassak", "dalyarak", "dalyarrak", "dangalak", "dassagi",
			"diktim", "dildo", "dingil", "dingilini", "dinsiz", "dkerim", "domal", "domalan", "domaldı", "domaldın",
			"domalık", "domalıyor", "domalmak", "domalmış", "domalsın", "domalt", "domaltarak", "domaltıp", "domaltır", "domaltırım",
			"domaltip", "domaltmak", "dölü", "dönek", "düdük", "eben", "ebeni", "ebenin", "ebeninki", "ebleh",
			"ecdadını", "ecdadini", "embesil", "fahise", "fahişe", "feriştah", "ferre", "fuck", "fucker", "fuckin",
			"fucking", "gavad", "gavat", "gay", "geber", "geberik", "gebermek", "gebermiş", "gebertir", "gerızekalı",
			"geri zekalı", "gerizekalı", "gerizekali", "gerzek", "giberim", "giberler", "gibis", "gibiş", "gibmek", "gibtiler",
			"goddamn", "godoş", "godumun", "gotelek", "gotlalesi", "gotlu", "gotten", "gotundeki", "gotunden", "gotune",
			"gotunu", "gotveren", "goyiim", "goyum", "goyuyim", "goyyim", "göt", "göt deliği", "göt herif", "göt oğlanı",
			"göt veren", "göt verir", "götelek", "götlalesi", "götlek", "götoğlanı", "götoş", "götten", "götü", "götün",
			"götüne", "götüne koyim", "götünekoyim", "götünü", "götveren", "gtelek", "gtn", "gtnde", "gtnden", "gtne",
			"gtten", "gtveren", "has siktir", "hasiktir", "hassikome", "hassiktir", "hassittir", "haysiyetsiz", "hayvan herif", "hoşafı",
			"hödük", "hsktr", "huur", "ıbnelık", "ibina", "ibine", "ibinenin", "ibne", "ibnedir", "ibneleri",
			"ibnelik", "ibnelri", "ibneni", "ibnenin", "ibnerator", "ibnesi", "idiot", "idiyot", "imansz", "ipne",
			"iserim", "işerim", "itoğlu it", "kafam girsin", "kafasız", "kafasiz", "kahpe", "kahpenin", "kahpenin feryadı", "kaka",
			"kaltak", "kancık", "kancik", "kappe", "karhane", "kaşar", "kavat", "kavatn", "kaypak", "kayyum",
			"kerane", "kerhane", "kerhanelerde", "kevase", "kevaşe", "kevvase", "koca göt", "koduğmun", "koduğmunun", "kodumun",
			"kodumunun", "koduumun", "koyarm", "koyayım", "koyiim", "koyiiym", "koyim", "koyum", "koyyim", "krar",
			"kukudaym", "laciye boyadım", "lavuk", "liboş", "madafaka", "mal", "malafat", "malak", "manyak", "mcik",
			"meme", "memelerini", "mezveleli", "minaamcık", "mincikliyim", "mna", "monakkoluyum", "motherfucker", "mudik", "o. çocuğu",
			"oc", "ocuu", "ocuun", "oç", "OÇ", "oğlan", "oğlancı", "oğlu it", "oe", "œ",
			"oc", "oç orosbucocuu", "orospu", "orospu cocugu", "orospu çoc", "orospu çocuğu", "orospu çocuğudur", "orospu çocukları", "orospucocugu", "orospuçocuğu",
			"orospudur", "orospular", "orospunun", "orospunun evladı", "orospuydu", "orospuyuz", "orostoban", "orostopol", "orrospu", "oruspu",
			"oruspu çocuğu", "oruspuçocuğu", "osbir", "ossurduum", "ossurmak", "ossuruk", "osur", "osurduu", "osuruk", "osururum",
			"otuzbir", "öküz", "öşex", "patlak zar", "penis", "pezevek", "pezeven", "pezeveng", "pezevengi", "pezevengin evladı",
			"pezevenk", "pezo", "pic", "pici", "picler", "piç", "piç kurusu", "piçin oğlu", "piçler", "pipi",
			"pipiş", "pisliktir", "porno", "pussy", "puşt", "puşttur", "rahminde", "revizyonist", "sg", "s1kerim",
			"s1kerm", "s1krm", "sakso", "saksofon", "salaak", "salak", "saxo", "sekis", "serefsiz", "sevgi koyarım",
			"sevişelim", "sexs", "sıçarım", "sıçtığım", "sıecem", "sicarsin", "sie", "sik", "sikdi", "sikdiğim",
			"sike", "sikecem", "sikem", "siken", "sikenin", "siker", "sikerim", "sikerler", "sikersin", "sikertir",
			"sikertmek", "sikesen", "sikesicenin", "sikey", "sikeydim", "sikeyim", "sikeym", "siki", "sikicem", "sikici",
			"sikien", "sikienler", "sikiiim", "sikiiimmm", "sikiim", "sikiir", "sikiirken", "sikik", "sikil", "sikildiini",
			"sikilesice", "sikilmi", "sikilmie", "sikilmis", "sikilmiş", "sikilsin", "sikim", "sikimde", "sikimden", "sikime",
			"sikimi", "sikimiin", "sikimin", "sikimle", "sikimsonik", "sikimtrak", "sikin", "sikinde", "sikinden", "sikine",
			"sikini", "sikip", "sikis", "sikisek", "sikisen", "sikish", "sikismis", "sikiş", "sikişen", "sikişme",
			"sikitiin", "sikiyim", "sikiym", "sikiyorum", "sikkim", "sikko", "sikleri", "sikleriii", "sikli", "sikm",
			"sikmek", "sikmem", "sikmiler", "sikmisligim", "siksem", "sikseydin", "sikseyidin", "siksin", "siksinbaya", "siksinler",
			"siksiz", "siksok", "siksz", "sikt", "sikti", "siktigimin", "siktigiminin", "siktiğim", "siktiğimin", "siktiğiminin",
			"siktii", "siktiim", "siktiimin", "siktiiminin", "siktiler", "siktim", "siktimin", "siktiminin", "siktir", "siktir et",
			"siktir git", "siktir lan", "siktir ol git", "siktirgit", "siktirir", "siktiririm", "siktiriyor", "siktirolgit", "sittimin", "sittir",
			"skcem", "skecem", "skem", "sker", "skerim", "skerm", "skeyim", "skiim", "skik", "skim",
			"skime", "skmek", "sksin", "sksn", "sksz", "sktiimin", "sktrr", "skyim", "slaleni", "sokam",
			"sokarım", "sokarim", "sokarm", "sokarmkoduumun", "sokayım", "sokaym", "sokiim", "soktuğumunun", "sokuk", "sokum",
			"sokuş", "sokuyum", "soxum", "sulaleni", "sülaleni", "sülalenizi", "sürtük", "şerefsiz", "şıllık", "taaklarn",
			"taaklarna", "tarrakimin", "tasak", "tassak", "taşak", "taşşak", "tipini s.k", "tipinizi s.keyim", "tiyniyat", "toplarm",
			"topsun", "totoş", "vajina", "vajinanı", "veled", "veled i zina", "veledizina", "verdiimin", "weled", "weledizina",
			"whore", "xikeyim", "yaaraaa", "yalama", "yalarım", "yalarun", "yaraaam", "yarak", "yaraksız", "yaraktr",
			"yaram", "yaraminbasi", "yaramn", "yararmorospunun", "yarra", "yarraaaa", "yarraak", "yarraam", "yarraamı", "yarragi",
			"yarragimi", "yarragina", "yarragindan", "yarragm", "yarrağ", "yarrağım", "yarrağımı", "yarraimin", "yarrak", "yarram",
			"yarramin", "yarraminbaşı", "yarramn", "yarran", "yarrana", "yarrrak", "yavak", "yavş", "yavşak", "yavşaktır",
			"yavuşak", "yılışık", "yilisik", "yogurtlayam", "yoğurtlayam", "yrrak", "zıkkımım", "zibidi", "zigsin", "zikeyim",
			"zikiiim", "zikiim", "zikik", "zikim", "ziksiiin", "ziksiin", "zulliyetini", "zviyetini", "卐", "卍"
		};

		public static string[] maleRandomName = new string[103]
		{
			"Liam", "Noah", "Oliver", "Elijah", "William", "James", "Benjamin", "Lucas", "Henry", "Alexander",
			"Daniel", "Matthew", "Sebastian", "Jack", "Owen", "Theodore", "Samuel", "Joseph", "David", "John",
			"Arthur", "Thomas", "Leo", "Mohamed", "Aiden", "Gabriel", "Santiago", "Dylan", "Luke", "Mateo",
			"Mason", "Julian", "Jayden", "Leo", "Christian", "Wyatt", "Levi", "Isaac", "Ryan", "Nathan",
			"Muhammad", "Carlos", "Elias", "Max", "Leon", "Arjun", "Oscar", "Adam", "Lucas", "Jay",
			"Zachary", "Luis", "Alex", "Kamil", "Nikolai", "Rafael", "Marco", "Diego", "Ezra", "Pablo",
			"Omar", "Ethan", "Lucas", "Kevin", "Paul", "Ivan", "Kian", "Miguel", "Tomas", "Daniel",
			"Jonas", "Martin", "Victor", "Nino", "Rohan", "Samuel", "Luca", "Ravi", "Haruto", "Seth",
			"Jorge", "Karim", "Emil", "Eren", "Mateo", "Nikola", "Teo", "Reza", "Viktor", "Andrei",
			"Pierre", "Ali", "Enzo", "Alexandre", "Tiago", "Juan", "Silas", "Yusuf", "Leandro", "Hamza",
			"Iskander", "Musa", "Azat"
		};

		public static string[] femaleRandomName = new string[100]
		{
			"Olivia", "Emma", "Ava", "Sophia", "Isabella", "Mia", "Charlotte", "Amelia", "Harper", "Evelyn",
			"Abigail", "Ella", "Avery", "Scarlett", "Grace", "Lily", "Zoe", "Victoria", "Riley", "Aria",
			"Luna", "Layla", "Nora", "Mila", "Aubrey", "Hannah", "Addison", "Leah", "Camila", "Sofia",
			"Lucy", "Anna", "Stella", "Aurora", "Natalie", "Everly", "Emilia", "Naomi", "Ariana", "Lillian",
			"Brooklyn", "Savannah", "Zoey", "Penelope", "Maya", "Madeline", "Paisley", "Madison", "Ellie", "Bella",
			"Eva", "Claire", "Skylar", "Hazel", "Violet", "Samantha", "Mackenzie", "Allison", "Alexa", "Aaliyah",
			"Elena", "Gabriella", "Claire", "Sarah", "Ariana", "Leilani", "Alice", "Alexis", "Maria", "Isabelle",
			"Eliana", "Valentina", "Autumn", "Quinn", "Caroline", "Kennedy", "Josephine", "Bella", "Sophie", "Alyssa",
			"Ivy", "Jade", "Lyla", "Ruby", "Eva", "Serenity", "Willow", "Everleigh", "Cora", "Kaylee",
			"Julia", "Lydia", "Vivian", "Madeline", "Piper", "Reagan", "Rylee", "Clara", "Arianna", "Sienna"
		};

		private static string[] veryBadWords = new string[2] { "卐", "卍" };

		private HashSet<string> badWordsHashset;

		private void LazyInitializeBadWordsHashset()
		{
			badWordsHashset = wordsToRemove.ToHashSet();
		}

		public string Filter(string text)
		{
			if (badWordsHashset == null)
			{
				LazyInitializeBadWordsHashset();
			}
			string[] array = text.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = (string[])array.Clone();
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = new string(array2[i].Where((char c) => char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)).ToArray());
				if (!badWordsHashset.Contains(array2[i].ToLower()))
				{
					continue;
				}
				string text2 = array2[i];
				StringBuilder stringBuilder = new StringBuilder("***");
				if (text2.Length > 3)
				{
					for (int num = 3; num < text2.Length; num++)
					{
						stringBuilder.Append("*");
					}
				}
				array[i] = stringBuilder.ToString();
			}
			string input = string.Join(" ", array);
			input = RemoveRichTextFast(input);
			string[] array3 = veryBadWords;
			foreach (string oldValue in array3)
			{
				input = input.Replace(oldValue, "***");
			}
			return input;
		}

		private string RemoveRichTextFast(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (char c in input)
			{
				switch (c)
				{
				case '<':
					flag = true;
					continue;
				case '>':
					flag = false;
					continue;
				}
				if (!flag)
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
