#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D screenTexture;
uniform float strength;

void main() {
    vec2 offset = strength * vec2(0.001, 0.001) * vec2(0.5 - TexCoords.x, TexCoords.y - 0.5);

    vec4 color = texture(screenTexture, TexCoords + offset);

    FragColor = color;
}
