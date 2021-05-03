namespace Josing
{
    public static class JsonValid
    {

        /// <summary>
        /// �ж�һ���ַ����ǲ��ǺϷ���json�ַ���
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool IsJson(string json)
        {
            int errIndex;
            return IsJson(json, out errIndex);
        }

        private static bool IsJson(string json, out int errIndex)
        {
            errIndex = 0;
            if (IsJsonStart(ref json))
            {
                CharState cs = new CharState();
                char c;
                for (int i = 0; i < json.Length; i++)
                {
                    c = json[i];
                    if (SetCharState(c, ref cs) && cs.childrenStart)//���ùؼ�����״̬��
                    {
                        string item = json.Substring(i);
                        int err;
                        int length = GetValueLength(item, true, out err);
                        cs.childrenStart = false;
                        if (err > 0)
                        {
                            errIndex = i + err;
                            return false;
                        }
                        i = i + length - 1;
                    }
                    if (cs.isError)
                    {
                        errIndex = i;
                        return false;
                    }
                }

                return !cs.arrayStart && !cs.jsonStart;
            }
            return false;
        }

        /// <summary>
        /// �Ƿ���json��ͷ���ַ���
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static bool IsJsonStart(ref string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                json = json.Trim('\r', '\n', ' ');
                if (json.Length > 1)
                {
                    char s = json[0];
                    char e = json[json.Length - 1];
                    return (s == '{' && e == '}') || (s == '[' && e == ']');
                }
            }
            return false;
        }

        /// <summary>
        /// ��ȡֵ�ĳ��ȣ���JsonֵǶ����"{"��"["��ͷʱ��
        /// </summary>
        private static int GetValueLength(string json, bool breakOnErr, out int errIndex)
        {
            errIndex = 0;
            int len = 0;
            if (!string.IsNullOrEmpty(json))
            {
                CharState cs = new CharState();
                char c;
                for (int i = 0; i < json.Length; i++)
                {
                    c = json[i];
                    if (!SetCharState(c, ref cs))//���ùؼ�����״̬��
                    {
                        if (!cs.jsonStart && !cs.arrayStart)//json�������ֲ������飬���˳���
                        {
                            break;
                        }
                    }
                    else if (cs.childrenStart)//�����ַ���ֵ״̬�¡�
                    {
                        int length = GetValueLength(json.Substring(i), breakOnErr, out errIndex);//�ݹ���ֵ������һ�����ȡ�����
                        cs.childrenStart = false;
                        cs.valueStart = 0;
                        //cs.state = 0;
                        i = i + length - 1;
                    }
                    if (breakOnErr && cs.isError)
                    {
                        errIndex = i;
                        return i;
                    }
                    if (!cs.jsonStart && !cs.arrayStart)//��¼��ǰ����λ�á�
                    {
                        len = i + 1;//���ȱ�����+1
                        break;
                    }
                }
            }
            return len;
        }
        /// <summary>
        /// �ַ�״̬
        /// </summary>
        private class CharState
        {
            internal bool jsonStart = false;//�� "{"��ʼ��...
            internal bool setDicValue = false;// ���������ֵ�ֵ�ˡ�
            internal bool escapeChar = false;//��"\"ת����ſ�ʼ��
                                             /// <summary>
                                             /// ���鿪ʼ������һ��ͷ���㡿��ֵǶ�׵��ԡ�childrenStart������ʶ��
                                             /// </summary>
            internal bool arrayStart = false;//��"[" ���ſ�ʼ��
            internal bool childrenStart = false;//�Ӽ�Ƕ�׿�ʼ�ˡ�
                                                /// <summary>
                                                /// ��0 ��ʼ״̬���� ������,�����š�����1 ����������ð�š�
                                                /// </summary>
            internal int state = 0;

            /// <summary>
            /// ��-1 ȡֵ��������0 δ��ʼ����1 �����ſ�ʼ����2 �����ſ�ʼ����3 ˫���ſ�ʼ��
            /// </summary>
            internal int keyStart = 0;
            /// <summary>
            /// ��-1 ȡֵ��������0 δ��ʼ����1 �����ſ�ʼ����2 �����ſ�ʼ����3 ˫���ſ�ʼ��
            /// </summary>
            internal int valueStart = 0;
            internal bool isError = false;//�Ƿ��﷨����

            internal void CheckIsError(char c)//ֻ����һ��������ΪGetLength��ݹ鵽ÿһ�������
            {
                if (keyStart > 1 || valueStart > 1)
                {
                    return;
                }
                //ʾ�� ["aa",{"bbbb":123,"fff","ddd"}] 
                switch (c)
                {
                    case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                        isError = jsonStart && state == 0;//�ظ���ʼ���� ͬʱ����ֵ����
                        break;
                    case '}':
                        isError = !jsonStart || (keyStart != 0 && state == 0);//�ظ��������� ���� ��ǰ����{"aa"}����������{}
                        break;
                    case '[':
                        isError = arrayStart && state == 0;//�ظ���ʼ����
                        break;
                    case ']':
                        isError = !arrayStart || jsonStart;//�ظ���ʼ���� ���� Json δ����
                        break;
                    case '"':
                    case '\'':
                        isError = !(jsonStart || arrayStart); //json �����鿪ʼ��
                        if (!isError)
                        {
                            //�ظ���ʼ [""",{"" "}]
                            isError = (state == 0 && keyStart == -1) || (state == 1 && valueStart == -1);
                        }
                        if (!isError && arrayStart && !jsonStart && c == '\'')//['aa',{}]
                        {
                            isError = true;
                        }
                        break;
                    case ':':
                        isError = !jsonStart || state == 1;//�ظ����֡�
                        break;
                    case ',':
                        isError = !(jsonStart || arrayStart); //json �����鿪ʼ��
                        if (!isError)
                        {
                            if (jsonStart)
                            {
                                isError = state == 0 || (state == 1 && valueStart > 1);//�ظ����֡�
                            }
                            else if (arrayStart)//["aa,] [,]  [{},{}]
                            {
                                isError = keyStart == 0 && !setDicValue;
                            }
                        }
                        break;
                    case ' ':
                    case '\r':
                    case '\n'://[ "a",\r\n{} ]
                    case '\0':
                    case '\t':
                        break;
                    default: //ֵ��ͷ����
                        isError = (!jsonStart && !arrayStart) || (state == 0 && keyStart == -1) || (valueStart == -1 && state == 1);//
                        break;
                }
                //if (isError)
                //{

                //}
            }
        }
        /// <summary>
        /// �����ַ�״̬(����true��Ϊ�ؼ��ʣ�����false��Ϊ��ͨ�ַ�����
        /// </summary>
        private static bool SetCharState(char c, ref CharState cs)
        {
            cs.CheckIsError(c);
            switch (c)
            {
                case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                    #region ������
                    if (cs.keyStart <= 0 && cs.valueStart <= 0)
                    {
                        cs.keyStart = 0;
                        cs.valueStart = 0;
                        if (cs.jsonStart && cs.state == 1)
                        {
                            cs.childrenStart = true;
                        }
                        else
                        {
                            cs.state = 0;
                        }
                        cs.jsonStart = true;//��ʼ��
                        return true;
                    }
                    #endregion
                    break;
                case '}':
                    #region �����Ž���
                    if (cs.keyStart <= 0 && cs.valueStart < 2 && cs.jsonStart)
                    {
                        cs.jsonStart = false;//����������
                        cs.state = 0;
                        cs.keyStart = 0;
                        cs.valueStart = 0;
                        cs.setDicValue = true;
                        return true;
                    }
                    // cs.isError = !cs.jsonStart && cs.state == 0;
                    #endregion
                    break;
                case '[':
                    #region �����ſ�ʼ
                    if (!cs.jsonStart)
                    {
                        cs.arrayStart = true;
                        return true;
                    }
                    else if (cs.jsonStart && cs.state == 1)
                    {
                        cs.childrenStart = true;
                        return true;
                    }
                    #endregion
                    break;
                case ']':
                    #region �����Ž���
                    if (cs.arrayStart && !cs.jsonStart && cs.keyStart <= 2 && cs.valueStart <= 0)//[{},333]//����������
                    {
                        cs.keyStart = 0;
                        cs.valueStart = 0;
                        cs.arrayStart = false;
                        return true;
                    }
                    #endregion
                    break;
                case '"':
                case '\'':
                    #region ����
                    if (cs.jsonStart || cs.arrayStart)
                    {
                        if (cs.state == 0)//key�׶�,�п���������["aa",{}]
                        {
                            if (cs.keyStart <= 0)
                            {
                                cs.keyStart = (c == '"' ? 3 : 2);
                                return true;
                            }
                            else if ((cs.keyStart == 2 && c == '\'') || (cs.keyStart == 3 && c == '"'))
                            {
                                if (!cs.escapeChar)
                                {
                                    cs.keyStart = -1;
                                    return true;
                                }
                                else
                                {
                                    cs.escapeChar = false;
                                }
                            }
                        }
                        else if (cs.state == 1 && cs.jsonStart)//ֵ�׶α�����Json��ʼ�ˡ�
                        {
                            if (cs.valueStart <= 0)
                            {
                                cs.valueStart = (c == '"' ? 3 : 2);
                                return true;
                            }
                            else if ((cs.valueStart == 2 && c == '\'') || (cs.valueStart == 3 && c == '"'))
                            {
                                if (!cs.escapeChar)
                                {
                                    cs.valueStart = -1;
                                    return true;
                                }
                                else
                                {
                                    cs.escapeChar = false;
                                }
                            }

                        }
                    }
                    #endregion
                    break;
                case ':':
                    #region ð��
                    if (cs.jsonStart && cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 0)
                    {
                        if (cs.keyStart == 1)
                        {
                            cs.keyStart = -1;
                        }
                        cs.state = 1;
                        return true;
                    }
                    // cs.isError = !cs.jsonStart || (cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 1);
                    #endregion
                    break;
                case ',':
                    #region ���� //["aa",{aa:12,}]

                    if (cs.jsonStart)
                    {
                        if (cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 1)
                        {
                            cs.state = 0;
                            cs.keyStart = 0;
                            cs.valueStart = 0;
                            //if (cs.valueStart == 1)
                            //{
                            //    cs.valueStart = 0;
                            //}
                            cs.setDicValue = true;
                            return true;
                        }
                    }
                    else if (cs.arrayStart && cs.keyStart <= 2)
                    {
                        cs.keyStart = 0;
                        //if (cs.keyStart == 1)
                        //{
                        //    cs.keyStart = -1;
                        //}
                        return true;
                    }
                    #endregion
                    break;
                case ' ':
                case '\r':
                case '\n'://[ "a",\r\n{} ]
                case '\0':
                case '\t':
                    if (cs.keyStart <= 0 && cs.valueStart <= 0) //cs.jsonStart && 
                    {
                        return true;//�����ո�
                    }
                    break;
                default: //ֵ��ͷ����
                    if (c == '\\') //ת�����
                    {
                        if (cs.escapeChar)
                        {
                            cs.escapeChar = false;
                        }
                        else
                        {
                            cs.escapeChar = true;
                            return true;
                        }
                    }
                    else
                    {
                        cs.escapeChar = false;
                    }
                    if (cs.jsonStart || cs.arrayStart) // Json �����鿪ʼ�ˡ�
                    {
                        if (cs.keyStart <= 0 && cs.state == 0)
                        {
                            cs.keyStart = 1;//�����ŵ�
                        }
                        else if (cs.valueStart <= 0 && cs.state == 1 && cs.jsonStart)//ֻ��Json��ʼ����ֵ��
                        {
                            cs.valueStart = 1;//�����ŵ�
                        }
                    }
                    break;
            }
            return false;
        }

    }
}

