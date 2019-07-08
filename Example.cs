using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples
{
    public class Example : MonoBehaviour
    {
        private GCSpeechRecognition _speechRecognition;

        private Button _startRecordButton,
                       _stopRecordButton;

        private Image _speechRecognitionState;

        private Text _speechRecognitionResult;

        private Toggle _isRuntimeDetectionToggle;

        private Dropdown _languageDropdown;

        private InputField _contextPhrases;

        //public bool flag = false;

        private void Start()
        {
            _speechRecognition = GCSpeechRecognition.Instance;
            _speechRecognition.RecognitionSuccessEvent += RecognitionSuccessEventHandler;
            _speechRecognition.NetworkRequestFailedEvent += SpeechRecognizedFailedEventHandler;
            _speechRecognition.LongRecognitionSuccessEvent += LongRecognitionSuccessEventHandler;

            _startRecordButton = transform.Find("Canvas/Button_StartRecord").GetComponent<Button>();
            _stopRecordButton = transform.Find("Canvas/Button_StopRecord").GetComponent<Button>();

            _speechRecognitionState = transform.Find("Canvas/Image_RecordState").GetComponent<Image>();

            _speechRecognitionResult = transform.Find("Canvas/Text_Result").GetComponent<Text>();

            _isRuntimeDetectionToggle = transform.Find("Canvas/Toggle_IsRuntime").GetComponent<Toggle>();

            _languageDropdown = transform.Find("Canvas/Dropdown_Language").GetComponent<Dropdown>();

            _contextPhrases = transform.Find("Canvas/InputField_SpeechContext").GetComponent<InputField>();

            _startRecordButton.onClick.AddListener(StartRecordButtonOnClickHandler);
            _stopRecordButton.onClick.AddListener(StopRecordButtonOnClickHandler);

            _speechRecognitionState.color = Color.white;
            _startRecordButton.interactable = true;
            _stopRecordButton.interactable = false;

            _languageDropdown.ClearOptions();

            for (int i = 0; i < Enum.GetNames(typeof(Enumerators.LanguageCode)).Length; i++)
            {
                _languageDropdown.options.Add(new Dropdown.OptionData(((Enumerators.LanguageCode)i).ToString()));
            }

            _languageDropdown.onValueChanged.AddListener(LanguageDropdownOnValueChanged);

            _languageDropdown.value = _languageDropdown.options.IndexOf(_languageDropdown.options.Find(x => x.text == Enumerators.LanguageCode.en_GB.ToString()));

        }

        /*public bool Getflag()
        {

        }*/

        private void OnDestroy()
        {
            _speechRecognition.RecognitionSuccessEvent -= RecognitionSuccessEventHandler;
            _speechRecognition.NetworkRequestFailedEvent -= SpeechRecognizedFailedEventHandler;
            _speechRecognition.LongRecognitionSuccessEvent -= LongRecognitionSuccessEventHandler;
        }


        private void StartRecordButtonOnClickHandler()//録音ボタンON
        {
            _startRecordButton.interactable = false;//録音ボタン無効
            _stopRecordButton.interactable = true;//ストップボタン有効
            _speechRecognitionState.color = Color.red;//赤色
            _speechRecognitionResult.text = string.Empty;//テキスト空
            _speechRecognition.StartRecord(_isRuntimeDetectionToggle.isOn);//録音開始
        }

        private void StopRecordButtonOnClickHandler()//ストップボタンON
        {
            ApplySpeechContextPhrases();

            _stopRecordButton.interactable = false;//ストップボタン無効
            _speechRecognitionState.color = Color.yellow;//黄色
            _speechRecognition.StopRecord();//録音終了
        }

        private void LanguageDropdownOnValueChanged(int value)
        {
            _speechRecognition.SetLanguage((Enumerators.LanguageCode)value);//言語選択をした
        }

        private void ApplySpeechContextPhrases()
        {
            string[] phrases = _contextPhrases.text.Trim().Split(","[0]);

            if (phrases.Length > 0)
                _speechRecognition.SetContext(new List<string[]>() { phrases });
        }

        private void SpeechRecognizedFailedEventHandler(string obj, long requestIndex)//聞き取れなかった
        {
            _speechRecognitionResult.text = "Speech Recognition failed with error: " + obj;

            if (!_isRuntimeDetectionToggle.isOn)//チェックボックスOFF
            {
                _speechRecognitionState.color = Color.green;//緑色
                _startRecordButton.interactable = true;//録音ボタン有効
                _stopRecordButton.interactable = false;//ストップボタン無効
            }
        }

        private void RecognitionSuccessEventHandler(RecognitionResponse obj, long requestIndex)//聞き取れたとき
        {
            Debug.Log("RecognitionSuccess");//メッセージ表示
            if (obj != null && obj.results.Length > 0)
            {
                File.AppendAllText("Assets/Resources/test.txt", obj.results[0].alternatives[0].transcript);
                File.AppendAllText("Assets/Resources/test.txt", Environment.NewLine);
                //flag = true;

            }
            if (!_isRuntimeDetectionToggle.isOn)//チェックボックスOFF
            {
                _startRecordButton.interactable = true;//開始ボタン有効
                _speechRecognitionState.color = Color.green;//緑色
            }

            if (obj != null && obj.results.Length > 0)
            {
                _speechRecognitionResult.text = "Speech Recognition succeeded! Detected Most useful: " + obj.results[0].alternatives[0].transcript;    

                var words = obj.results[0].alternatives[0].words;

                if (words != null)
                {
                    string times = string.Empty;

                    foreach (var item in obj.results[0].alternatives[0].words)//要素を1つずつ取り出す
                        times += "<color=green>" + item.word + "</color> -  start: " + item.startTime + "; end: " + item.endTime + "\n";//言葉＋録音開始時間＋録音終了時間

                    _speechRecognitionResult.text += "\n" + times;
                }

                string other = "\nDetected alternative: ";

                foreach (var result in obj.results)
                {
                    foreach (var alternative in result.alternatives)
                    {
                        if (obj.results[0].alternatives[0] != alternative)
                            other += alternative.transcript + ", ";
                    }
                }

                _speechRecognitionResult.text += other;
            }
            else
            {
                _speechRecognitionResult.text = "Speech Recognition succeeded! Words are no detected.";
            }
        }

        private void LongRecognitionSuccessEventHandler(OperationResponse operation, long index)
        {
            if (!_isRuntimeDetectionToggle.isOn)
            {
                _startRecordButton.interactable = true;
                _speechRecognitionState.color = Color.green;
            }

            if (operation != null && operation.response.results.Length > 0)
            {
                _speechRecognitionResult.text = "Long Speech Recognition succeeded! Detected Most useful: " + operation.response.results[0].alternatives[0].transcript;

                string other = "\nDetected alternative: ";

                foreach (var result in operation.response.results)
                {
                    foreach (var alternative in result.alternatives)
                    {
                        if (operation.response.results[0].alternatives[0] != alternative)
                            other += alternative.transcript + ", ";
                    }
                }

                _speechRecognitionResult.text += other;
                _speechRecognitionResult.text += "\nTime for the recognition: " + 
                    (operation.metadata.lastUpdateTime - operation.metadata.startTime).TotalSeconds + "s";
            }
            else
            {
                _speechRecognitionResult.text = "Speech Recognition succeeded! Words are no detected.";
            }
        }
    }
}