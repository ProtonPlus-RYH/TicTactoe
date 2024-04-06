using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LanguageManager : MonoSingleton<LanguageManager>
{
    private StringTable localizationTable;
    void Start()
    {
        if (LocalizationSettings.AvailableLocales.Locales.Count > 0)
        {
            GetLocalizationTable();
        }
        else
        {
            LocalizationSettings.InitializationOperation.Completed += OnLocalizationInitialized;
        }
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    public void languageSelect(int languageOrder)//0Ϊ���У� 1ΪӢ��
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageOrder];
    }
    
    private void OnLocalizationInitialized(AsyncOperationHandle<LocalizationSettings> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Localization initialized successfully!");
            GetLocalizationTable();
        }
        else
        {
            Debug.LogError("Localization initialization failed.");
        }
    }
    public void OnLocaleChanged(Locale locale)
    {
        GetLocalizationTable();
    }
    public void GetLocalizationTable()
    {
        localizationTable = LocalizationSettings.StringDatabase.GetTable("LocalizationTable");
    }
    public string GetLocalizedString(string localizedTableKey)
    {
        return localizationTable.GetEntry(localizedTableKey).GetLocalizedString();
    }
}