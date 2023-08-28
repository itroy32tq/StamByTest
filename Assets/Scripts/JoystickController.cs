using Net;
using UnityEngine.EventSystems;
using UnityEngine;

public class JoystickController : MonoBehaviour, IDragHandler, IEndDragHandler
{
    //Радиус, на который будет двигаться джойстик
    [SerializeField] private float _range = 1;
    //Название джойстика
    public string StickName { get; set; }
    //место, где появился джойстик(чтоб считать радиус
    Vector2 _startPosition;
    //делаем локальную переменную для нашего джойстика
    //чтоб постоянно не обращаться к нему через
    //transform.GetChild(0);
    Transform _joystickTransform;
    void Start()
    {
        //В начале работы необходимо зарегестрировать джойстик
#if UNITY_EDITOR
        gameObject.SetActive(false);
#endif

        //Запоминаем изначальное положение джойстика
        _startPosition = transform.GetChild(0).position;
        //Запоминаем джойстик в локальной переменной
        _joystickTransform = transform.GetChild(0);
    }
    //Метод, который срабатывает, когда объект пытаются двигать
    public void OnDrag(PointerEventData Data)
    {


        //Ппроверяем расстояние до точки, куда джойстик пытаются перетащить
        if (Vector2.Distance(_startPosition, Data.position) < _range)
        {
            //Если расстояние приемлимое, то переносим наш джойстик туда, где сейчас палец
            _joystickTransform.position = Data.position;
            //и заносим данные о джойстике в наш класс
            //где (Data.position.x - _startPosition.x) / _range - значение от -1 до 1
            AndroidIosInput.SetJoystickValue(StickName, new Vector2((Data.position.x - _startPosition.x) / _range, (Data.position.y - _startPosition.y) / _range));
        }
        else
        {
            //запоминаем смещение по x и y от начала
            float deltaX = Data.position.x - _startPosition.x;
            float deltaY = Data.position.y - _startPosition.y;
            //Ищем точку на тригонометрической окружности относительно смещения
            //И чтоб была на краю нашего _range
            Vector2 state = Vector2.ClampMagnitude(new Vector2(deltaX, deltaY), _range);
            //перемещаем джойстик в эту точку
            _joystickTransform.position = state + _startPosition;
            //Устанавливаем значение джойстика
            //где state.x / _range - значение от -1 до 1
            AndroidIosInput.SetJoystickValue(StickName, new Vector2(state.x / _range, state.y / _range));
        }
    }
    public void OnEndDrag(PointerEventData Data)
    {
        //Обнуляем наш джойстик
        AndroidIosInput.SetJoystickValue(StickName, Vector2.zero);
        //Возвращаем его на прежнее место
        _joystickTransform.position = _startPosition;
    }
    private void OnDestroy()
    {
        //Удаляем запись о джойстике в случае смены сцены или удаления джойстика
        AndroidIosInput.RegisterJoystick(StickName);
    }
}