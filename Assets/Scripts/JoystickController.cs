using Net;
using UnityEngine.EventSystems;
using UnityEngine;

public class JoystickController : MonoBehaviour, IDragHandler, IEndDragHandler
{
    //������, �� ������� ����� ��������� ��������
    [SerializeField] private float _range = 1;
    //�������� ���������
    public string StickName { get; set; }
    //�����, ��� �������� ��������(���� ������� ������
    Vector2 _startPosition;
    //������ ��������� ���������� ��� ������ ���������
    //���� ��������� �� ���������� � ���� �����
    //transform.GetChild(0);
    Transform _joystickTransform;
    void Start()
    {
        //� ������ ������ ���������� ���������������� ��������
#if UNITY_EDITOR
        gameObject.SetActive(false);
#endif

        //���������� ����������� ��������� ���������
        _startPosition = transform.GetChild(0).position;
        //���������� �������� � ��������� ����������
        _joystickTransform = transform.GetChild(0);
    }
    //�����, ������� �����������, ����� ������ �������� �������
    public void OnDrag(PointerEventData Data)
    {


        //���������� ���������� �� �����, ���� �������� �������� ����������
        if (Vector2.Distance(_startPosition, Data.position) < _range)
        {
            //���� ���������� ����������, �� ��������� ��� �������� ����, ��� ������ �����
            _joystickTransform.position = Data.position;
            //� ������� ������ � ��������� � ��� �����
            //��� (Data.position.x - _startPosition.x) / _range - �������� �� -1 �� 1
            AndroidIosInput.SetJoystickValue(StickName, new Vector2((Data.position.x - _startPosition.x) / _range, (Data.position.y - _startPosition.y) / _range));
        }
        else
        {
            //���������� �������� �� x � y �� ������
            float deltaX = Data.position.x - _startPosition.x;
            float deltaY = Data.position.y - _startPosition.y;
            //���� ����� �� ������������������ ���������� ������������ ��������
            //� ���� ���� �� ���� ������ _range
            Vector2 state = Vector2.ClampMagnitude(new Vector2(deltaX, deltaY), _range);
            //���������� �������� � ��� �����
            _joystickTransform.position = state + _startPosition;
            //������������� �������� ���������
            //��� state.x / _range - �������� �� -1 �� 1
            AndroidIosInput.SetJoystickValue(StickName, new Vector2(state.x / _range, state.y / _range));
        }
    }
    public void OnEndDrag(PointerEventData Data)
    {
        //�������� ��� ��������
        AndroidIosInput.SetJoystickValue(StickName, Vector2.zero);
        //���������� ��� �� ������� �����
        _joystickTransform.position = _startPosition;
    }
    private void OnDestroy()
    {
        //������� ������ � ��������� � ������ ����� ����� ��� �������� ���������
        AndroidIosInput.RegisterJoystick(StickName);
    }
}