using System;
using System.Collections.Generic;
using System.IO;
using Palmmedia.ReportGenerator.Core.Common;
using UnityEngine;
using System.Text.Json;
using System.ComponentModel;

namespace DefaultNamespace
{
	public class PositionSaver : MonoBehaviour
	{
        [Serializable]
        public struct Data
		{
			public Vector3 Position;
			public float Time;
		}

        [Tooltip("Для заполнения этого поля используйте контекстное меню (Create File)")]
		[SerializeField, ReadOnly]
        private TextAsset _json;

        [SerializeField, HideInInspector]
        private List<Data> _records;
        public List<Data> Records
        {
            get => _records;
            private set => _records = value;
        }
        //public List<Data> Records { get; private set; }


        private void Awake()
		{
            //todo comment: Что будет, если в теле этого условия не сделать выход из метода?
            //Без возврата выполнение продолжится, что вызовет ошибку при попытке десериализации
            if (_json == null)
			{
				gameObject.SetActive(false);
				Debug.LogError("Please, create TextAsset and add in field _json");
				return;
			}
			
			JsonUtility.FromJsonOverwrite(_json.text, this);
            //todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
            //Защита от исключения(NullReferenceException), если JSON не содержит данных

            if (Records == null)
				Records = new List<Data>(10);
		}

		private void OnDrawGizmos()
		{
            //todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
            //Предотвращают ошибки при отсутствии данных
            if (Records == null || Records.Count == 0) return;
			var data = Records;
			var prev = data[0].Position;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(prev, 0.3f);
            //todo comment: Почему итерация начинается не с нулевого элемента?
            //0 элемент уже обработан
            for (int i = 1; i < data.Count; i++)
			{
				var curr = data[i].Position;
				Gizmos.DrawWireSphere(curr, 0.3f);
				Gizmos.DrawLine(prev, curr);
				prev = curr;
			}
		}
		
#if UNITY_EDITOR
		[ContextMenu("Create File")]
		private void CreateFile()
		{
            //todo comment: Что происходит в этой строке?
            //Создание файла в папке Assets
            var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
            //todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав) 
            //Освобождает ресурсы файла для доступа Unity
            stream.Dispose();
			UnityEditor.AssetDatabase.Refresh();
			//В Unity можно искать объекты по их типу, для этого используется префикс "t:"
			//После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
			var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
			foreach (var guid in guids)
			{
				//Этой командой можно получить путь к ассету через его гуид
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				//Этой командой можно загрузить сам ассет
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                //todo comment: Для чего нужны эти проверки?
                //Поиск конкретного файла по имени и валидация
                if (asset != null && asset.name == "Path")
				{
					_json = asset;
					UnityEditor.EditorUtility.SetDirty(this);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
                    //todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
                    //Экономия ресурсов после нахождения нужного ассета
                    return;
				}
			}
		}

		private void OnDestroy()
		{
#if UNITY_EDITOR

            if (_json == null)
            {
                Debug.LogError("_json NULL");
                return;
            }

			var json = JsonUtility.ToJson(this, true);
			

            string path = UnityEditor.AssetDatabase.GetAssetPath(_json);
            File.WriteAllText(path, json);

            UnityEditor.AssetDatabase.Refresh();
#endif

        }
        
#endif
    }
}