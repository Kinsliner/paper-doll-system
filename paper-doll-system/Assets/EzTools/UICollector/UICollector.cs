using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Ez.Tool
{
    [Serializable]
    public class UIAsset
    {
        public string realKey;
        public string groupName;
        public string keyName;
        public UnityEngine.Object asset;
    }

    public class UICollector : MonoBehaviour
    {
        public int collectorID = -1;
        public string guid = string.Empty;
        public string groupName;
        public List<UIAsset> UIAsset = new List<UIAsset>();

        private Dictionary<string, UIAsset> UIAssetsPair = new Dictionary<string, UIAsset>();
        private Dictionary<string, List<Action>> onClicks = new Dictionary<string, List<Action>>();
        private Dictionary<string, List<Action>> onChecks = new Dictionary<string, List<Action>>();
        private Dictionary<string, List<Action<int>>> onValueChanged_dropdown = new Dictionary<string, List<Action<int>>>();
        private Dictionary<string, List<Action<bool>>> onValueChanged_toggle = new Dictionary<string, List<Action<bool>>>();
        private Dictionary<string, List<Action<string>>> onValueChanged_inputField = new Dictionary<string, List<Action<string>>>();
        private Dictionary<string, List<Action<float>>> onValueChanged_slider = new Dictionary<string, List<Action<float>>>();
        private Dictionary<string, List<Action<float>>> onValueChanged_scrollbar = new Dictionary<string, List<Action<float>>>();
        private Dictionary<string, List<Action<Vector2>>> onValueChanged_scrollRect = new Dictionary<string, List<Action<Vector2>>>();
        private bool isInit = false;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            if (isInit == false)
            {
                InitAssets();
                InitEvents();
                isInit = true;
            }
        }

        private void InitAssets()
        {
            foreach (var ui in UIAsset)
            {
                UIAssetsPair.Add(ui.realKey, ui);
            }
        }

        private void InitEvents()
        {
            foreach (var item in UIAsset)
            {
                string key = item.realKey;
                var ui = item.asset;

                if (ui is Button)
                {
                    (ui as Button).onClick.AddListener(() => OnButtonClick(key));
                }
                if (ui is Dropdown)
                {
                    (ui as Dropdown).onValueChanged.AddListener((v) => OnValueChanged(key, v));
                }
                if (ui is Toggle)
                {
                    (ui as Toggle).onValueChanged.AddListener((v) => OnValueChanged(key, v));
                }
                if (ui is InputField)
                {
                    (ui as InputField).onValueChanged.AddListener((v) => OnValueChanged(key, v));
                }
                if (ui is Slider)
                {
                    (ui as Slider).onValueChanged.AddListener((v) => OnSliderChanged(key, v));
                }
                if (ui is Scrollbar)
                {
                    (ui as Scrollbar).onValueChanged.AddListener((v) => OnScrollbarChanged(key, v));
                }
                if(ui is ScrollRect)
                {
                    (ui as ScrollRect).onValueChanged.AddListener((v) => OnScrollRectChanged(key, v));
                }
            }
        }

        private void OnButtonClick(string key)
        {
            if (onClicks.ContainsKey(key))
            {
                foreach (var action in onClicks[key])
                {
                    action?.Invoke();
                }
            }
            if (onChecks.ContainsKey(key))
            {
                foreach (var action in onChecks[key].ToArray())
                {
                    action?.Invoke();
                }
            }
        }

        private void OnValueChanged(string key, int value)
        {
            if (onValueChanged_dropdown.ContainsKey(key))
            {
                foreach (var action in onValueChanged_dropdown[key])
                {
                    action?.Invoke(value);
                }
            }
        }

        private void OnValueChanged(string key, bool value)
        {
            if (onValueChanged_toggle.ContainsKey(key))
            {
                foreach (var action in onValueChanged_toggle[key])
                {
                    action?.Invoke(value);
                }
            }
            if (value == true)
            {
                if (onChecks.ContainsKey(key))
                {
                    foreach (var action in onChecks[key].ToArray())
                    {
                        action?.Invoke();
                    }
                }
            }
        }

        private void OnValueChanged(string key, string value)
        {
            if (onValueChanged_inputField.ContainsKey(key))
            {
                foreach (var action in onValueChanged_inputField[key])
                {
                    action?.Invoke(value);
                }
            }
        }

        private void OnSliderChanged(string key, float value)
        {
            if (onValueChanged_slider.ContainsKey(key))
            {
                foreach (var action in onValueChanged_slider[key])
                {
                    action?.Invoke(value);
                }
            }
        }

        private void OnScrollbarChanged(string key, float value)
        {
            if (onValueChanged_scrollbar.ContainsKey(key))
            {
                foreach (var action in onValueChanged_scrollbar[key])
                {
                    action?.Invoke(value);
                }
            }
        }

        private void OnScrollRectChanged(string key, Vector2 value)
        {
            if (onValueChanged_scrollRect.ContainsKey(key))
            {
                foreach (var action in onValueChanged_scrollRect[key])
                {
                    action?.Invoke(value);
                }
            }
        }

        public void BindOnClick(string key, Action action)
        {
            if (onClicks.ContainsKey(key) == false)
            {
                onClicks.Add(key, new List<Action>());
            }
            if (onClicks[key].Contains(action) == false)
            {
                onClicks[key].Add(action);
            }
        }

        /// <summary>
        /// OnCheck會在Button被按下以及Toggle的值被改為true的時候觸發
        /// </summary>
        public void BindOnCheck(string key, Action action)
        {
            if (onChecks.ContainsKey(key) == false)
            {
                onChecks.Add(key, new List<Action>());
            }
            if (onChecks[key].Contains(action) == false)
            {
                onChecks[key].Add(action);
            }
        }

        public void BindOnValueChange(string key, Action<int> action)
        {
            if (onValueChanged_dropdown.ContainsKey(key) == false)
            {
                onValueChanged_dropdown.Add(key, new List<Action<int>>());
            }
            if (onValueChanged_dropdown[key].Contains(action) == false)
            {
                onValueChanged_dropdown[key].Add(action);
            }
        }

        public void BindOnValueChange(string key, Action<bool> action)
        {
            if (onValueChanged_toggle.ContainsKey(key) == false)
            {
                onValueChanged_toggle.Add(key, new List<Action<bool>>());
            }
            if (onValueChanged_toggle[key].Contains(action) == false)
            {
                onValueChanged_toggle[key].Add(action);
            }
        }

        public void BindOnValueChange(string key, Action<string> action)
        {
            if (onValueChanged_inputField.ContainsKey(key) == false)
            {
                onValueChanged_inputField.Add(key, new List<Action<string>>());
            }
            if (onValueChanged_inputField[key].Contains(action) == false)
            {
                onValueChanged_inputField[key].Add(action);
            }
        }

        public void BindOnSliderChange(string key, Action<float> action)
        {
            if (onValueChanged_slider.ContainsKey(key) == false)
            {
                onValueChanged_slider.Add(key, new List<Action<float>>());
            }
            if (onValueChanged_slider[key].Contains(action) == false)
            {
                onValueChanged_slider[key].Add(action);
            }
        }

        public void BindOnScrollbarChange(string key, Action<float> action)
        {
            if (onValueChanged_scrollbar.ContainsKey(key) == false)
            {
                onValueChanged_scrollbar.Add(key, new List<Action<float>>());
            }
            if (onValueChanged_scrollbar[key].Contains(action) == false)
            {
                onValueChanged_scrollbar[key].Add(action);
            }
        }

        public void BindOnValueChange(string key, Action<Vector2> action)
        {
            if (onValueChanged_scrollRect.ContainsKey(key) == false)
            {
                onValueChanged_scrollRect.Add(key, new List<Action<Vector2>>());
            }
            if (onValueChanged_scrollRect[key].Contains(action) == false)
            {
                onValueChanged_scrollRect[key].Add(action);
            }
        }

        public void Unbind(string key, Action action)
        {
            if (onClicks.ContainsKey(key))
            {
                if (onClicks[key].Contains(action))
                {
                    onClicks[key].Remove(action);
                }
            }
            if (onChecks.ContainsKey(key))
            {
                if (onChecks[key].Contains(action))
                {
                    onChecks[key].Remove(action);
                }
            }
        }

        public void Unbind(string key, Action<int> action)
        {
            if (onValueChanged_dropdown.ContainsKey(key))
            {
                if (onValueChanged_dropdown[key].Contains(action))
                {
                    onValueChanged_dropdown[key].Remove(action);
                }
            }
        }

        public void Unbind(string key, Action<bool> action)
        {
            if (onValueChanged_toggle.ContainsKey(key))
            {
                if (onValueChanged_toggle[key].Contains(action))
                {
                    onValueChanged_toggle[key].Remove(action);
                }
            }
        }

        public void Unbind(string key, Action<string> action)
        {
            if (onValueChanged_inputField.ContainsKey(key))
            {
                if (onValueChanged_inputField[key].Contains(action))
                {
                    onValueChanged_inputField[key].Remove(action);
                }
            }
        }

        public void Unbind(string key, Action<float> action)
        {
            if (onValueChanged_slider.ContainsKey(key))
            {
                if (onValueChanged_slider[key].Contains(action))
                {
                    onValueChanged_slider[key].Remove(action);
                }
            }
            if (onValueChanged_scrollbar.ContainsKey(key))
            {
                if (onValueChanged_scrollbar[key].Contains(action))
                {
                    onValueChanged_scrollbar[key].Remove(action);
                }
            }
        }

        public void Unbind(string key, Action<Vector2> action)
        {
            if (onValueChanged_scrollRect.ContainsKey(key))
            {
                if (onValueChanged_scrollRect[key].Contains(action))
                {
                    onValueChanged_scrollRect[key].Remove(action);
                }
            }
        }

        public void UnbindAll(string key)
        {
            if (onClicks.ContainsKey(key))
            {
                onClicks[key].Clear();
                onClicks.Remove(key);
            }
            if (onChecks.ContainsKey(key))
            {
                onChecks[key].Clear();
                onChecks.Remove(key);
            }
            if (onValueChanged_dropdown.ContainsKey(key))
            {
                onValueChanged_dropdown[key].Clear();
                onValueChanged_dropdown.Remove(key);
            }
            if (onValueChanged_toggle.ContainsKey(key))
            {
                onValueChanged_toggle[key].Clear();
                onValueChanged_toggle.Remove(key);
            }
            if (onValueChanged_inputField.ContainsKey(key))
            {
                onValueChanged_inputField[key].Clear();
                onValueChanged_inputField.Remove(key);
            }
            if (onValueChanged_slider.ContainsKey(key))
            {
                onValueChanged_slider[key].Clear();
                onValueChanged_slider.Remove(key);
            }
            if (onValueChanged_scrollbar.ContainsKey(key))
            {
                onValueChanged_scrollbar[key].Clear();
                onValueChanged_scrollbar.Remove(key);
            }
            if (onValueChanged_scrollRect.ContainsKey(key))
            {
                onValueChanged_scrollRect[key].Clear();
                onValueChanged_scrollRect.Remove(key);
            }
        }

        public string GetText(string key)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is Text)
                {
                    return (uiAsset.asset as Text).text;
                }
                if (uiAsset.asset is InputField)
                {
                    return (uiAsset.asset as InputField).text;
                }
                if (uiAsset.asset is TMP_Text)
                {
                    return (uiAsset.asset as TMP_Text).text;
                }
                if (uiAsset.asset is TMP_InputField)
                {
                    return (uiAsset.asset as TMP_InputField).text;
                }
            }
            
            return string.Empty;
        }

        public void SetText(string key, string text)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is Text)
                {
                    (uiAsset.asset as Text).text = text;
                }
                if (uiAsset.asset is InputField)
                {
                    (uiAsset.asset as InputField).text = text;
                }
                if (uiAsset.asset is TMP_Text)
                {
                    (uiAsset.asset as TMP_Text).text = text;
                }
                if (uiAsset.asset is TMP_InputField)
                {
                    (uiAsset.asset as TMP_InputField).text = text;
                }
            }
        }

        public Color GetColor(string key)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is Graphic)
                {
                    return (uiAsset.asset as Graphic).color;
                }
            }
            return default;
        }

        public void SetColor(string key, Color color)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is Graphic)
                {
                    (uiAsset.asset as Graphic).color = color;
                }
            }
        }

        public Sprite GetImage(string key)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is Image)
                {
                    return (uiAsset.asset as Image).sprite;
                }
            }
            return default;
        }

        public void SetImage(string key, Sprite image)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if(uiAsset != null)
            {
                if (uiAsset.asset is Image)
                {
                    (uiAsset.asset as Image).sprite = image;
                }
            }
        }

        /// <summary>
        /// 取得Image的fillAmount值
        /// </summary>
        public float GetFill(string key)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is Image)
                {
                    return (uiAsset.asset as Image).fillAmount;
                }
            }
            return 0;
        }

        /// <summary>
        /// 操作Image的fillAmount值
        /// </summary>
        public void SetFill(string key, float fillAmount)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is Image)
                {
                    (uiAsset.asset as Image).fillAmount = fillAmount;
                }
            }
        }

        /// <summary>
        /// 取得Slider和Scrollbar的Value值
        /// </summary>
        public void GetValue(string key, out float value)
        {
            UIAsset uiAsset = GetUIAsset(key);
            value = 0;
            if (uiAsset != null)
            {
                if (uiAsset.asset is Slider)
                {
                    value = (uiAsset.asset as Slider).value;
                }
                if (uiAsset.asset is Scrollbar)
                {
                    value = (uiAsset.asset as Scrollbar).value;
                }
            }
        }

        /// <summary>
        /// 操作Slider和Scrollbar的Value值
        /// </summary>
        public void SetValue(string key, float value)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is Slider)
                {
                    (uiAsset.asset as Slider).value = value;
                }
                if (uiAsset.asset is Scrollbar)
                {
                    (uiAsset.asset as Scrollbar).value = value;
                }
                if (uiAsset.asset is Image)
                {
                    (uiAsset.asset as Image).fillAmount = value;
                }
            }
        }

        /// <summary>
        /// 取得Dropdown的Value值
        /// </summary>
        public void GetValue(string key, out int value)
        {
            UIAsset uiAsset = GetUIAsset(key);
            value = 0;
            if (uiAsset != null)
            {
                if (uiAsset.asset is Dropdown)
                {
                    value = (uiAsset.asset as Dropdown).value;
                }
            }
        }

        /// <summary>
        /// 操作Dropdown的Value值
        /// </summary>
        public void SetValue(string key, int value)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is Dropdown)
                {
                    (uiAsset.asset as Dropdown).value = value;
                }
            }
        }


        /// <summary>
        /// 取得Toggle的isOn值
        /// </summary>
        public void GetValue(string key, out bool value)
        {
            UIAsset uiAsset = GetUIAsset(key);
            value = false;
            if (uiAsset != null)
            {
                if (uiAsset.asset is Toggle)
                {
                    value = (uiAsset.asset as Toggle).isOn;
                }
            }
        }

        /// <summary>
        /// 操作Toggle的isOn值
        /// </summary>
        public void SetValue(string key, bool value)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is Toggle)
                {
                    (uiAsset.asset as Toggle).isOn = value;
                }
            }
        }

        /// <summary>
        /// 取得Graphic和CanvasGroup的Alpha值
        /// </summary>
        public float GetAlpha(string key)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is CanvasGroup)
                {
                    return (uiAsset.asset as CanvasGroup).alpha;
                }
                if (uiAsset.asset is Graphic)
                {
                    Graphic g = uiAsset.asset as Graphic;
                    return g.color.a;
                }
            }
            return 0;
        }

        /// <summary>
        /// 操作Graphic和CanvasGroup的Alpha值
        /// </summary>
        public void SetAlpha(string key, float alpha)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is CanvasGroup)
                {
                    (uiAsset.asset as CanvasGroup).alpha = alpha;
                }
                if (uiAsset.asset is Graphic)
                {
                    Graphic g = uiAsset.asset as Graphic;
                    Color c = g.color;
                    g.color = new Color(c.r, c.g, c.b, alpha);
                }
            }
        }

        public void SetInteractable(string key, bool interactable)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is Selectable)
                {
                    (uiAsset.asset as Selectable).interactable = interactable;
                }
            }
        }

        public void Active(string key) => SetActive(key, true);
        public void Deactive(string key) => SetActive(key, false);
        public void SetActive(string key, bool isActive)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is GameObject)
                {
                    (uiAsset.asset as GameObject).SetActive(isActive);
                }
                if (uiAsset.asset is Component)
                {
                    (uiAsset.asset as Component).gameObject.SetActive(isActive);
                }
            }
        }

        public void Enable(string key) => SetEnabled(key, true);
        public void Disable(string key) => SetEnabled(key, false);
        public void SetEnabled(string key, bool isEnbled)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is Behaviour)
                {
                    (uiAsset.asset as Behaviour).enabled = isEnbled;
                }
            }
        }

        public T GetAsset<T>(string key) where T : UnityEngine.Object
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is T)
                {
                    return (uiAsset.asset as T);
                }
                if (uiAsset.asset is GameObject)
                {
                    if ((uiAsset.asset as GameObject).TryGetComponent(out T comp))
                    {
                        return comp;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 取得該Asset上的Component
        /// </summary>
        public T GetAssetComponent<T>(string key) where T : Component
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is T)
                {
                    return (uiAsset.asset as T);
                }
                if (uiAsset.asset is Component)
                {
                    if ((uiAsset.asset as Component).gameObject.TryGetComponent(out T comp))
                    {
                        return comp;
                    }
                }
                if (uiAsset.asset is GameObject)
                {
                    if ((uiAsset.asset as GameObject).TryGetComponent(out T comp))
                    {
                        return comp;
                    }
                }
            }
            return null;
        }

        public RectTransform GetRectTransform(string key)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is RectTransform)
                {
                    return uiAsset.asset as RectTransform;
                }
                if (uiAsset.asset is GameObject)
                {
                    return (uiAsset.asset as GameObject).GetComponent<RectTransform>();
                }
                if (uiAsset.asset is Component)
                {
                    return (uiAsset.asset as Component).GetComponent<RectTransform>();
                }
            }
            return null;
        }

        public GameObject GetGameObject(string key)
        {
            UIAsset uiAsset = GetUIAsset(key);
            if (uiAsset != null)
            {
                if (uiAsset.asset is GameObject)
                {
                    return uiAsset.asset as GameObject;
                }
                if (uiAsset.asset is Component)
                {
                    return (uiAsset.asset as Component).gameObject;
                }
            }
            return null;
        }

        public UIAsset GetUIAsset(string key)
        {
            if (UIAssetsPair.ContainsKey(key))
            {
                return UIAssetsPair[key];
            }
            else
            {
                return UIAsset.Find(p => p.realKey == key);
            }
        }
    }
}

