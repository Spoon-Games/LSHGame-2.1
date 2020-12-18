using UnityEngine;


namespace LSHGame.PlayerN
{
    [System.Serializable]
    public class InputButton
    {
        [SerializeField]
        private float beforeConDeltaTime;
        [SerializeField]
        private float afterConDeltaTime;

        [SerializeField]
        private bool needRepress = false;

        [SerializeField]
        private float resetTimer;

        private float beforeConTime = float.NegativeInfinity;
        private float afterConTime = float.NegativeInfinity;
        private float conIndex = 0;
        private float lastActiveTime = float.NegativeInfinity;

        private bool wasRealeasedSinceActivate = true;

        private bool wasButtonPressedAndActive = false;

        //public InputButton(string keyCode,float beforeTime,float afterTime)
        //{
        //    this.keyCode = keyCode;
        //    this.beforeConDeltaTime = beforeTime;
        //    this.afterConDeltaTime = afterTime;
        //}

        public bool Check(bool buttonDown, bool condition, int conIndex = 0)
        {
            bool tmp = false;
            return Check(buttonDown, condition, ref tmp, conIndex);
        }

        public bool Check(bool buttonDown, bool condition, ref bool wasReleased, int conIndex = 0)
        {
            if (CheckRaw2(buttonDown, condition, conIndex))
            {
                if (!buttonDown)
                    wasReleased = true;
                else
                    wasButtonPressedAndActive = true;

                return true;
            }

            if (!buttonDown && wasButtonPressedAndActive)
            {
                wasReleased = true;
                wasButtonPressedAndActive = false;
            }

            return false;
        }

        public void Reset()
        {
            beforeConTime = float.NegativeInfinity;
            afterConTime = float.NegativeInfinity;
            conIndex = 0;
            lastActiveTime = float.NegativeInfinity;

            wasRealeasedSinceActivate = true;

            wasButtonPressedAndActive = false;
        }


        private bool CheckRaw2(bool buttonDown, bool condition, int conIndex)
        {
            condition &= Time.time - lastActiveTime >= resetTimer;

            wasRealeasedSinceActivate |= !buttonDown;
            if (needRepress && !wasRealeasedSinceActivate)
                buttonDown = false;

            if (CheckRaw(buttonDown, condition, conIndex))
            {
                lastActiveTime = Time.time;
                wasRealeasedSinceActivate = false;
                return true;
            }
            return false;
        }

        private bool CheckRaw(bool buttonDown, bool condition, int conIndex)
        {
            if (condition && buttonDown)
            {
                afterConTime = float.NegativeInfinity;
                beforeConTime = float.NegativeInfinity;
                return true;
            }

            if (buttonDown && afterConTime >= Time.time && this.conIndex == conIndex)
            {
                afterConTime = float.NegativeInfinity;
                beforeConTime = float.NegativeInfinity;
                return true;
            }

            if (condition && beforeConTime >= Time.time)
            {
                afterConTime = float.NegativeInfinity;
                beforeConTime = float.NegativeInfinity;
                return true;
            }

            if (condition && !buttonDown)
            {
                beforeConTime = float.NegativeInfinity;
                afterConTime = Time.time + afterConDeltaTime;
                this.conIndex = conIndex;
            }

            if (buttonDown && !condition)
            {
                afterConTime = float.NegativeInfinity;
                beforeConTime = Time.time + beforeConDeltaTime;
            }

            return false;
        }
    }
}
