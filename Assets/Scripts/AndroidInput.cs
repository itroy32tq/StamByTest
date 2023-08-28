using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Net
{
    //Делаем класс статическим, чтобы его элементы были доступны из любого места
    public static class AndroidIosInput
    {
        //Словарь для того, чтоб хранить данные о джойстике по его имени
        static Dictionary<string, Vector2> JoySticks;

        static AndroidIosInput()
        {
            //при загрузке этого класса, мы создаем новый словарь
            JoySticks = new Dictionary<string, Vector2>();
        }
        //Метод регистрации. Когда джойстик появляется, он регистрируется
        public static string RegisterJoystick(string Name)
        {
            //Проверяем этот джойстик на наличие
            if (JoySticks.ContainsKey(Name))
                throw new System.Exception("Joystick " + Name + " already registered");
            //Если его нет, то регистрируем наш джойстик
            JoySticks.Add(Name, Vector2.zero);

            return Name;
        }
        //Получение значения джойстика
        public static Vector2 GetJoystickValue(string Name)
        {
            //Проверяем его наличие
            if (!JoySticks.ContainsKey(Name))
                throw new System.Exception("Joystick " + Name + " didn't registered");
            //Возвращаем значение джойстика, если он есть
            return JoySticks[Name];
        }

        //Установление значения джойстика
        public static void SetJoystickValue(string Name, Vector2 Value)
        {
            if (!JoySticks.ContainsKey(Name))
                throw new System.Exception("Joystick " + Name + " didn't registered");
            //Устанавливаем значение для джойстика
            JoySticks[Name] = Value;
        }
        //Удаление джойстика
        public static void RemoveJoystick(string Name)
        {
            JoySticks.Remove(Name);
        }
    }
}
