using Il2CppMonomiPark.SlimeRancher.Script.Util;
using UnityEngine.Localization;

namespace LimesFashionPods
{
    public static class TranslationPatcher
    {
        private static readonly Dictionary<string, Dictionary<string, string>> AddedTranslations = new Dictionary<string, Dictionary<string, string>>();

        public static LocalizedString AddTranslation(string table, string key, string localized)
        {
            var stringTable = LocalizationUtil.GetTable(table);
            if (stringTable == null)
                throw new NullReferenceException("Table is null");
            
            if (!AddedTranslations.TryGetValue(table, out var patched))
            {
                var dictionary = new Dictionary<string, string>();
                patched = dictionary;
                AddedTranslations.Add(table, dictionary);
            }
            if (patched.ContainsKey(key))
            {
                Entrypoint.Logger.Msg($"Translation Key {key} for table {table} is already taken by another mod! Overwriting");
                patched[key] = localized;
                var localizedStringTableEntry = stringTable.GetEntry(key);
                localizedStringTableEntry.Value = localized;
                return new LocalizedString(stringTable.SharedData.TableCollectionName, localizedStringTableEntry.SharedEntry.Id);
            }
            patched.TryAdd(key, localized);

            var stringTableEntry = stringTable.AddEntry(key, localized);
            return new LocalizedString(stringTable.SharedData.TableCollectionName, stringTableEntry.SharedEntry.Id);
        }
    }
}