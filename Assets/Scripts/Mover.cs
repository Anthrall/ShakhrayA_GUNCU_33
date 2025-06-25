using System.Collections;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField]
    private Vector3 _start;
    [SerializeField]
    private Vector3 _end;
    [SerializeField]
	private float _moveTime = 1f;
	[SerializeField]
	private float _delayTime = 2f;
	//[SerializeField]
	//private Vector3[] _positions;

    private bool _isInitialized;
    private Vector3 _globalStart;
    private Vector3 _globalEnd;

    private Rigidbody _rb;
    private IEnumerator Start()
    {
        //if(_positions.Length < 2) yield break;
        //int prev = 0, curr = 1;
        //var time = 0f;
        //var transform = this.transform;
        //while(true)
        //{
        //	transform.position = Vector3.Lerp(_positions[prev], _positions[curr], time / _moveTime);
        //	time += Time.deltaTime;
        //	if(time >= _moveTime)
        //	{
        //		time = 0f;
        //		prev = curr;
        //		curr = (curr + 1) % _positions.Length;
        //		yield return new WaitForSeconds(_delayTime);
        //	}

        //	yield return null;
        //}
       
        _rb = GetComponent<Rigidbody>(); 
        
        _isInitialized = true;
        UpdateGlobalPoints();

        Vector3 globalStart = transform.TransformPoint(_start);
        Vector3 globalEnd = transform.TransformPoint(_end);

        while (true)
        {
            yield return MoveToPoint(globalStart, globalEnd);
            yield return new WaitForSeconds(_delayTime);
            yield return MoveToPoint(globalEnd, globalStart);
            yield return new WaitForSeconds(_delayTime);
        }
    }

    private IEnumerator MoveToPoint(Vector3 from, Vector3 to)
    {
        float journeyLength = Vector3.Distance(from, to);
        float startTime = Time.time;

        while (Vector3.Distance(_rb.position, to) > 0.01f)
        {
            float distanceCovered = (Time.time - startTime) * _moveTime;
            float fraction = distanceCovered / journeyLength;
            _rb.MovePosition(Vector3.Lerp(from, to, fraction));
            yield return new WaitForFixedUpdate();
        }
        _rb.MovePosition(to);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 startPoint, endPoint;

        if (Application.isPlaying && _isInitialized)
        {
            // ¬ режиме игры используем сохраненные точки
            startPoint = _globalStart;
            endPoint = _globalEnd;
        }
        else
        {
            // ¬ редакторе преобразуем локальные координаты
            startPoint = transform.TransformPoint(_start);
            endPoint = transform.TransformPoint(_end);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPoint, 0.3f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPoint, 0.3f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPoint, endPoint);
   
        
    }

    private void UpdateGlobalPoints()
    {
        _globalStart = transform.TransformPoint(_start);
        _globalEnd = transform.TransformPoint(_end);
    }
}
