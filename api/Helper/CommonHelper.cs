namespace npm.api.API.Helper
{
    using System.Text.RegularExpressions;

    public class CommonHelper
    {
        /// <summary>
        /// 檢查密碼強度
        /// </summary>
        /// <param name="password">密碼</param>
        /// <param name="minLength">最小長度</param>
        /// <param name="maxLength">最大長度(null則略過檢查)</param>
        /// <param name="hasNumber">必須至少有一個數字</param>
        /// <param name="hasLetter">必須至少有一個英文字母(不區分大小寫)</param>
        /// <param name="hasDiffCaseLetter">必須至少有一個大寫字母及一個小寫字母</param>
        /// <param name="hasSpecialChar">必須至少有一個特殊符號</param>
        /// <returns>是否通過檢查</returns>
        public static bool CheckPasswordComplexity(string password, int minLength = 8,
            int? maxLength = null, bool hasNumber = true, bool hasLetter = true, bool hasDiffCaseLetter = true, bool hasSpecialChar = false)
        {
            return CheckPasswordComplexity(password, out string _, minLength,
                maxLength, hasNumber, hasLetter, hasDiffCaseLetter, hasSpecialChar);
        }

        /// <summary>
        /// 檢查密碼強度
        /// </summary>
        /// <param name="password">密碼</param>
        /// <param name="errMsg">錯誤訊息</param>
        /// <param name="minLength">最小長度</param>
        /// <param name="maxLength">最大長度(null則略過檢查)</param>
        /// <param name="hasNumber">必須至少有一個數字</param>
        /// <param name="hasLetter">必須至少有一個英文字母(不區分大小寫)</param>
        /// <param name="hasDiffCaseLetter">必須至少有一個大寫字母及一個小寫字母</param>
        /// <param name="hasSpecialChar">必須至少有一個特殊符號</param>
        /// <returns>是否通過檢查</returns>
        public static bool CheckPasswordComplexity(string password, out string errMsg, int minLength = 8,
            int? maxLength = null, bool hasNumber = true, bool hasLetter = true, bool hasDiffCaseLetter = false, bool hasSpecialChar = false)
        {
            errMsg = "";

            if (string.IsNullOrEmpty(password))
            {
                errMsg = "密碼不得為空！";
                return false;
            }

            if (password.Length < minLength)
            {
                errMsg = $"密碼長度至少為「{minLength}」！";
                return false;
            }

            if (maxLength.HasValue && password.Length > maxLength)
            {
                errMsg = $"密碼長度至多為「{maxLength}」！";
                return false;
            }

            if (hasNumber && !Regex.IsMatch(password, @"\d+"))
            {
                errMsg = "密碼必須包含數字！";
                return false;
            }

            bool lowerSuccess = Regex.IsMatch(password, @"[a-z]");
            bool upperSuccess = Regex.IsMatch(password, @"[A-Z]");

            if (hasLetter)
            {
                if (!(lowerSuccess || upperSuccess))
                {
                    errMsg = "密碼必須包含英文字母！";
                    return false;
                }
            }

            if (hasDiffCaseLetter)
            {
                if (!(lowerSuccess && upperSuccess))
                {
                    errMsg = "密碼必須包含大小寫英文字母！";
                    return false;
                }
            }

            if (hasSpecialChar && !Regex.IsMatch(password, @"[ !""#$%&'()*+,-.\/:;<=>?@\[\\\]^_`{|}~]"))
            {
                errMsg = "密碼必須包含特殊字元！";
                return false;
            }

            return errMsg == string.Empty;
        }

        /// <summary>
        /// 使用變數進行排序
        /// </summary>
        /// <param name="obj">物件</param>
        /// <param name="property">參數</param>
        /// <returns>物件</returns>
        public static object GetPropertyValue(object obj, string property)
        {
            System.Reflection.PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return propertyInfo.GetValue(obj, null);
        }
    }
}