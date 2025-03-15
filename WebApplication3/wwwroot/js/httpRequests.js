async function login(username, password, captchaInput, captchaId) {
    const response = await fetch('/User/Login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ username, password, captchaInput, captchaId })
    });

    if (response.ok) {
        var body = await response.json();
        if (body.status === 200) {
            return body;
        } else {
            Swal.fire('错误', body.message, 'error');
            throw new Error(body.message);
        }
    } else {
        Swal.fire('错误', '登录失败', 'error');
        throw new Error('登录失败');
    }
}

async function register(username, email, password, confirmPassword, captchaInput, captchaId) {
    const response = await fetch('/User/Register', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ username, email, password, confirmPassword, captchaInput, captchaId })
    });

    if (response.ok) {
        var body = await response.json();
        if (body.status === 200) {
            return body;
        } else {
            Swal.fire('错误', body.message, 'error');
            throw new Error(body.message);
        }
    } else {
        Swal.fire('错误', '注册失败', 'error');
        throw new Error('注册失败');
    }
}

async function GetCaptcha() {
    const response = await fetch('/api/Captcha', {
        method: 'GET',
    });

    if (response.ok) {
        var body = await response.json();
        if (body.status === 200) {
            return body;
        } else {
            Swal.fire('错误', body.message, 'error');
            throw new Error('获取验证码失败');
        }
    } else {
        Swal.fire('错误', '获取验证码失败', 'error');
        throw new Error('获取验证码失败');
    }
}
