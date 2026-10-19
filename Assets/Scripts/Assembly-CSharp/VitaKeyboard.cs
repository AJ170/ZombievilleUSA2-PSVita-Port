using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class VitaKeyboard : MonoBehaviour
{
	private static VitaKeyboard instance;

	public string text = string.Empty;

	public bool done;

	public bool wasCanceled;

	public bool active;

#if UNITY_PSP2 && !UNITY_EDITOR
	private void Awake()
	{
		Sony.Vita.Dialog.Main.Initialise();
		Sony.Vita.Dialog.Ime.OnGotIMEDialogResult += OnGotIMEDialogResult;
	}

	private void OnDestroy()
	{
		Sony.Vita.Dialog.Ime.OnGotIMEDialogResult -= OnGotIMEDialogResult;
	}

	private void Update()
	{
		// Pumps the plugin so dialog results get dispatched.
		Sony.Vita.Dialog.Main.Update();
	}

	private void OnGotIMEDialogResult(Sony.Vita.Dialog.Messages.PluginMessage msg)
	{
		Sony.Vita.Dialog.Ime.ImeDialogResult result = Sony.Vita.Dialog.Ime.GetResult();
		if (result.result == Sony.Vita.Dialog.Ime.EnumImeDialogResult.RESULT_OK)
		{
			text = result.text;
		}
		else
		{
			wasCanceled = true;
		}
		done = true;
		active = false;
	}

	private void ShowDialog(string title, string initialText, int maxLen)
	{
		Sony.Vita.Dialog.Ime.ImeDialogParams info = new Sony.Vita.Dialog.Ime.ImeDialogParams();
		info.supportedLanguages = Sony.Vita.Dialog.Ime.FlagsSupportedLanguages.LANGUAGE_ENGLISH_GB;
		info.languagesForced = true;
		info.type = Sony.Vita.Dialog.Ime.EnumImeDialogType.TYPE_DEFAULT;
		info.option = 0;
		info.canCancel = true;
		info.textBoxMode = Sony.Vita.Dialog.Ime.FlagsTextBoxMode.TEXTBOX_MODE_WITH_CLEAR;
		info.enterLabel = Sony.Vita.Dialog.Ime.EnumImeDialogEnterLabel.ENTER_LABEL_DEFAULT;
		info.maxTextLength = maxLen;
		info.title = title;
		info.initialText = initialText;
		Sony.Vita.Dialog.Ime.Open(info);
	}
#else
	private TouchScreenKeyboard fallback;

	private void Update()
	{
		if (!active || fallback == null)
		{
			return;
		}
		text = fallback.text;
		if (fallback.wasCanceled)
		{
			wasCanceled = true;
		}
		if (fallback.done || fallback.wasCanceled)
		{
			done = true;
			active = false;
		}
	}

	private void ShowDialog(string title, string initialText, int maxLen)
	{
		fallback = TouchScreenKeyboard.Open(initialText, TouchScreenKeyboardType.Default, false, false, false, false, title);
	}
#endif

	public static VitaKeyboard Open(string initialText, string title, int maxLen)
	{
		if (instance == null)
		{
			GameObject go = new GameObject("VitaKeyboard");
			DontDestroyOnLoad(go);
			instance = go.AddComponent<VitaKeyboard>();
		}
		instance.text = initialText;
		instance.done = false;
		instance.wasCanceled = false;
		instance.active = true;
		instance.ShowDialog(title, initialText, maxLen);
		return instance;
	}
}
