// 时间戳可以提供一个大致的创建时间顺序
// UUID可以确保在同一个时间戳内生成的标识符的唯一性
function generateTimestampedUUID() {
    // 获取当前时间的时间戳（毫秒）
    const timestamp = Date.now();

    // 生成UUID v4
    const uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        const r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });

    // 将时间戳和UUID拼接起来
    return `${timestamp}-${uuid}`.replace(/-/g, '');
}

async function SetRecord(UUID, URL, Host) {
    const response = await fetch('https://www.ao3.fun/api/Record/SetRecord', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ UUID, URL, Host })
    });

    if (response.ok) {
        return await response.json();
    } else {
        throw new Error('Registration failed');
    }
}
    
var url = document.URL;
var host = window.location.host;


//设置缓存
var uuid = localStorage.getItem('UUID-RECORD')
if (uuid == null) {
    uuid = generateTimestampedUUID()
    localStorage.setItem('UUID-RECORD', uuid);
}
console.log(uuid);
console.log(SetRecord(uuid, url, host));
