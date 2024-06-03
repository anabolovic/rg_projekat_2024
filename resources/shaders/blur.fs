#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D image;

uniform bool horizontal;
uniform float weight[5] = float[] (0.2270270270, 0.1945945946, 0.1216216216, 0.0540540541, 0.0162162162);

// Threshold for bright areas
uniform float brightnessThreshold = 1.0;

void main() {
    vec2 tex_offset = 1.0 / textureSize(image, 0);
    vec3 color = texture(image, TexCoords).rgb;
    vec3 result = max(color - vec3(brightnessThreshold), 0.0) * weight[0];

    if (horizontal) {
        for (int i = 1; i < 5; ++i) {
            vec3 sample1 = texture(image, TexCoords + vec2(tex_offset.x * i, 0.0)).rgb;
            vec3 sample2 = texture(image, TexCoords - vec2(tex_offset.x * i, 0.0)).rgb;
            result += max(sample1 - vec3(brightnessThreshold), 0.0) * weight[i];
            result += max(sample2 - vec3(brightnessThreshold), 0.0) * weight[i];
        }
    } else {
        for (int i = 1; i < 5; ++i) {
            vec3 sample1 = texture(image, TexCoords + vec2(0.0, tex_offset.y * i)).rgb;
            vec3 sample2 = texture(image, TexCoords - vec2(0.0, tex_offset.y * i)).rgb;
            result += max(sample1 - vec3(brightnessThreshold), 0.0) * weight[i];
            result += max(sample2 - vec3(brightnessThreshold), 0.0) * weight[i];
        }
    }

    FragColor = vec4(result, 1.0);
}