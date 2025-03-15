function generateAudioFingerprint() {
    return new Promise((resolve, reject) => {
        try {
            const audioContext = new (window.AudioContext || window.webkitAudioContext)();
            const oscillator = audioContext.createOscillator();
            const analyser = audioContext.createAnalyser();
            const gain = audioContext.createGain();
            const scriptProcessor = audioContext.createScriptProcessor(4096, 1, 1);

            gain.gain.value = 0; // 静音
            oscillator.type = 'triangle'; // 设置振荡器类型
            oscillator.connect(analyser);
            analyser.connect(scriptProcessor);
            scriptProcessor.connect(gain);
            gain.connect(audioContext.destination);

            scriptProcessor.onaudioprocess = () => {
                const frequencyData = new Uint8Array(analyser.frequencyBinCount);
                analyser.getByteFrequencyData(frequencyData);
                const fingerprint = frequencyData.reduce((acc, val) => acc + val, 0);
                resolve(fingerprint);
                // 关闭音频上下文
                audioContext.close();
            };

            oscillator.start(0);
        } catch (error) {
            reject(error);
        }
    });
}

// 使用示例
generateAudioFingerprint().then(fingerprint => {
    console.log('Audio Fingerprint:', fingerprint);
}).catch(error => {
    console.error('Error generating audio fingerprint:', error);
});


function generateWebGLFingerprint() {
    return new Promise((resolve, reject) => {
        try {
            const canvas = document.createElement('canvas');
            const gl = canvas.getContext('webgl') || canvas.getContext('experimental-webgl');

            if (!gl) {
                reject('WebGL not supported');
                return;
            }

            const vertexShaderSource = `
                attribute vec2 position;
                void main() {
                    gl_Position = vec4(position, 0.0, 1.0);
                }
            `;

            const fragmentShaderSource = `
                void main() {
                    gl_FragColor = vec4(0.0, 1.0, 0.0, 1.0);
                }
            `;

            const vertexShader = gl.createShader(gl.VERTEX_SHADER);
            gl.shaderSource(vertexShader, vertexShaderSource);
            gl.compileShader(vertexShader);

            const fragmentShader = gl.createShader(gl.FRAGMENT_SHADER);
            gl.shaderSource(fragmentShader, fragmentShaderSource);
            gl.compileShader(fragmentShader);

            const program = gl.createProgram();
            gl.attachShader(program, vertexShader);
            gl.attachShader(program, fragmentShader);
            gl.linkProgram(program);
            gl.useProgram(program);

            const vertexData = new Float32Array([
                -1.0, -1.0,
                1.0, -1.0,
                -1.0, 1.0,
                1.0, 1.0,
            ]);

            const vertexBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
            gl.bufferData(gl.ARRAY_BUFFER, vertexData, gl.STATIC_DRAW);

            const positionLocation = gl.getAttribLocation(program, 'position');
            gl.enableVertexAttribArray(positionLocation);
            gl.vertexAttribPointer(positionLocation, 2, gl.FLOAT, false, 0, 0);

            gl.clearColor(0.0, 0.0, 0.0, 1.0);
            gl.clear(gl.COLOR_BUFFER_BIT);
            gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4);

            const pixels = new Uint8Array(gl.drawingBufferWidth * gl.drawingBufferHeight * 4);
            gl.readPixels(0, 0, gl.drawingBufferWidth, gl.drawingBufferHeight, gl.RGBA, gl.UNSIGNED_BYTE, pixels);

            const fingerprint = pixels.reduce((acc, val) => acc + val, 0);
            resolve(fingerprint);
        } catch (error) {
            reject(error);
        }
    });
}

// 使用示例
generateWebGLFingerprint().then(fingerprint => {
    console.log('WebGL Fingerprint:', fingerprint);
}).catch(error => {
    console.error('Error generating WebGL fingerprint:', error);
});


function generateCanvasFingerprint() {
    return new Promise((resolve, reject) => {
        try {
            const canvas = document.createElement('canvas');
            const context = canvas.getContext('2d');

            if (!context) {
                reject('Canvas not supported');
                return;
            }

            // 设置画布尺寸
            canvas.width = 200;
            canvas.height = 50;

            // 绘制文本
            context.textBaseline = 'top';
            context.font = '16px Arial';
            context.textBaseline = 'alphabetic';
            context.fillStyle = '#f60';
            context.fillRect(125, 1, 62, 20);
            context.fillStyle = '#069';
            context.fillText('Hello, world!', 2, 15);
            context.fillStyle = 'rgba(102, 204, 0, 0.7)';
            context.fillText('Hello, world!', 4, 17);

            // 绘制图形
            context.strokeStyle = 'rgba(102, 204, 0, 0.7)';
            context.beginPath();
            context.arc(50, 50, 50, 0, Math.PI * 2, true);
            context.stroke();

            // 获取图像数据
            const imageData = context.getImageData(0, 0, canvas.width, canvas.height).data;
            const fingerprint = imageData.reduce((acc, val) => acc + val, 0);
            resolve(fingerprint);
        } catch (error) {
            reject(error);
        }
    });
}

// 使用示例
generateCanvasFingerprint().then(fingerprint => {
    console.log('Canvas Fingerprint:', fingerprint);
}).catch(error => {
    console.error('Error generating canvas fingerprint:', error);
});


