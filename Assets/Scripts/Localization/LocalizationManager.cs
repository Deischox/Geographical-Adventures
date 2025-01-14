using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoGame.Localization
{
	public class LocalizationManager : MonoBehaviour
	{
		public enum Language { English, French, German, Italian, Portuguese, Spanish }
		public static event System.Action onLanguageChanged;

		public LocalizedData[] languagePacks;

		Language activeLanguage;
		Dictionary<Language, Dictionary<string, string>> languageLookup;
		static LocalizationManager instance;

		void Awake()
		{
			instance = this;
			languageLookup = new Dictionary<Language, Dictionary<string, string>>();

			foreach (LocalizedData languagePack in languagePacks)
			{
				LocalizedString[] localizedStrings = languagePack.Load();
				foreach (var entry in localizedStrings)
				{
					Add(languagePack.language, entry);
				}
			}

		}

		void Add(Language language, LocalizedString entry)
		{

			if (!string.IsNullOrWhiteSpace(entry.text))
			{
				if (!languageLookup.ContainsKey(language))
				{
					languageLookup.Add(language, new Dictionary<string, string>());
				}

				languageLookup[language].Add(entry.id, entry.text);
			}
		}

		public void ChangeLanguage(Language language)
		{
			if (language != activeLanguage)
			{
				activeLanguage = language;
				onLanguageChanged?.Invoke();
			}
		}

		public static string Localize(string id)
		{
			// Use english as fallback if no localization for active language
			var lookup = instance.languageLookup[Language.English];

			// Try get localized text
			if (instance.languageLookup.ContainsKey(instance.activeLanguage))
			{
				lookup = instance.languageLookup[instance.activeLanguage];
				if (lookup.ContainsKey(id))
				{
					return lookup[id];
				}
			}

			// No entry found for active language; falling back to english
			if (lookup.ContainsKey(id))
			{
				return lookup[id];
			}
			// No entry found
			string missing = $"Missing entry: {id}";
			Debug.LogError(missing);
			return missing;
		}

		void OnDestroy()
		{
			onLanguageChanged = null;
		}

		void OnValidate()
		{

		}
	}
}