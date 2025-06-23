﻿using UnityEngine;

namespace DefaultNamespace
{
	
	[RequireComponent(typeof(PositionSaver))]
	public class EditorMover : MonoBehaviour
	{
		private PositionSaver _save;
		private float _currentDelay;

        //todo comment: Что произойдёт, если _delay > _duration?
        //ничего не запишется или запишется до начала движения
        [Range(0.2f, 1.0f)]
        [SerializeField] private float _delay = 0.5f;
        
		[Min(0.2f)]
        [SerializeField] private float _duration = 5f;

		private void Start()
		{
			//todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
			//тяжелая операция
			_save = GetComponent<PositionSaver>();
			_save.Records.Clear();

            //Проверка соотношения duration и delay
            if (_duration <= _delay)
            {
                _duration = _delay * 5f;
                Debug.LogWarning($"Длительность изменена на {_duration} чтобы быть больше значения задержки");
            }
        }

		private void Update()
		{
			_duration -= Time.deltaTime;
			if (_duration <= 0f)
			{
				enabled = false;
				Debug.Log($"<b>{name}</b> finished", this);
				return;
			}
			
			//todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
			//Потому что это константа
			_currentDelay -= Time.deltaTime;
			if (_currentDelay <= 0f)
			{
				_currentDelay = _delay;
				_save.Records.Add(new PositionSaver.Data
				{
					Position = transform.position,
                    //todo comment: Для чего сохраняется значение игрового времени?
                    // Чтобы знать, в какой момент времени была сделана запись позиции, наверное
                    Time = Time.time,
				});
			}
		}
	}
}