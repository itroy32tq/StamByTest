using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Net
{
    //������ ����� �����������, ����� ��� �������� ���� �������� �� ������ �����
    public static class AndroidIosInput
    {
        //������� ��� ����, ���� ������� ������ � ��������� �� ��� �����
        static Dictionary<string, Vector2> JoySticks;

        static AndroidIosInput()
        {
            //��� �������� ����� ������, �� ������� ����� �������
            JoySticks = new Dictionary<string, Vector2>();
        }
        //����� �����������. ����� �������� ����������, �� ��������������
        public static string RegisterJoystick(string Name)
        {
            //��������� ���� �������� �� �������
            if (JoySticks.ContainsKey(Name))
                throw new System.Exception("Joystick " + Name + " already registered");
            //���� ��� ���, �� ������������ ��� ��������
            JoySticks.Add(Name, Vector2.zero);

            return Name;
        }
        //��������� �������� ���������
        public static Vector2 GetJoystickValue(string Name)
        {
            //��������� ��� �������
            if (!JoySticks.ContainsKey(Name))
                throw new System.Exception("Joystick " + Name + " didn't registered");
            //���������� �������� ���������, ���� �� ����
            return JoySticks[Name];
        }

        //������������ �������� ���������
        public static void SetJoystickValue(string Name, Vector2 Value)
        {
            if (!JoySticks.ContainsKey(Name))
                throw new System.Exception("Joystick " + Name + " didn't registered");
            //������������� �������� ��� ���������
            JoySticks[Name] = Value;
        }
        //�������� ���������
        public static void RemoveJoystick(string Name)
        {
            JoySticks.Remove(Name);
        }
    }
}
