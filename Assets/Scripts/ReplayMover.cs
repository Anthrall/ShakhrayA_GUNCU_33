using System;
using UnityEngine;

namespace DefaultNamespace
{
	[RequireComponent(typeof(PositionSaver))]
	public class ReplayMover : MonoBehaviour
	{
		private PositionSaver _save;

		private int _index;
		private PositionSaver.Data _prev;
		private float _duration;

		private void Start()
		{
            ////todo comment: зачем нужны эти проверки?
            //Проверяет наличие компонента PositionSaver и непустоту записей
            if (!TryGetComponent(out _save) || _save.Records.Count == 0)
			{
				Debug.LogError("Records incorrect value", this);
                //todo comment: Для чего выключается этот компонент?
                //Чтобы вхолостую не вычислял, данных нет
                enabled = false;
			}
		}

		private void Update()
		{
			var curr = _save.Records[_index];
            //todo comment: Что проверяет это условие (с какой целью)? 
            //Проверяет, настало ли время переключиться на следующую точку
            if (Time.time > curr.Time)
			{
				_prev = curr;
				_index++;
                //todo comment: Для чего нужна эта проверка?
                //Проверяет завершение воспроизведения всех точек
                if (_index >= _save.Records.Count)
				{
					enabled = false;
					Debug.Log($"<b>{name}</b> finished", this);
				}
			}
            //todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
            //Расчет прогресса интерполяции между предыдущей и текущей точками

            var delta = (Time.time - _prev.Time) / (curr.Time - _prev.Time);
            //todo comment: Зачем нужна эта проверка?
            //Защита от деления на ноль и недопустимых значений
            if (float.IsNaN(delta)) delta = 0f;
            //todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
            // Вычисляется промежуточная позиция между предыдущей и текущей точкой:
            // - Используется линейная интерполяция (Lerp)
            // - _prev.Position - начальная точка интерполяции
            // - curr.Position - конечная точка интерполяции
            // - delta (0-1) определяет текущий прогресс перемещения между точками
            // - Результат присваивается позиции текущего объекта
            transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
		}
	}
}