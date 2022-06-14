
	
	private string trySolvingCaptcha()
        {
            string verifyCode = string.Empty;

            try
            {
                string sendUrl = string.Format("http://2captcha.com/in.php?key={0}&method=userrecaptcha&googlekey={1}&pageurl=sportingbet.com", Setting.Instance.captchaKey, Constants.googleKey);
                HttpResponseMessage responseMessageMain = httpClient.GetAsync(sendUrl).Result;
                responseMessageMain.EnsureSuccessStatusCode();

                string sendUrlString = responseMessageMain.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrEmpty(sendUrlString))
                    return verifyCode;

                if (!sendUrlString.Contains("OK|"))
                    return verifyCode;

                string captchaId = sendUrlString.Replace("OK|", string.Empty);
                if (string.IsNullOrEmpty(captchaId))
                    return verifyCode;

                string verifyUrl = string.Format("http://2captcha.com/res.php?key={0}&action=get&id={1}", Setting.Instance.captchaKey, captchaId);

                int requestCount = 0;
                while (requestCount < 20)
                {
                    Thread.Sleep(15000);
                    requestCount++;

                    HttpResponseMessage responseMessageVerify = httpClient.GetAsync(verifyUrl).Result;
                    responseMessageVerify.EnsureSuccessStatusCode();

                    string verifyUrlString = responseMessageVerify.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrEmpty(verifyUrlString))
                        continue;

                    if (!verifyUrlString.Contains("OK|") && verifyUrlString.Contains("CAPCHA_NOT_READY"))
                        continue;

                    verifyCode = verifyUrlString.Replace("OK|", string.Empty);
                    break;
                }

                return verifyCode;
            }
            catch (Exception e)
            {
                return verifyCode;
            }
        }