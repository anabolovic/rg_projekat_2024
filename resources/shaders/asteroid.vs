#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 TexCoords;
layout (location = 2) in vec3 aNormal;

out VS_OUT {
    vec3 FragPos;
    vec2 TexCoords;
    vec3 Normal;
} vs_out;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main() {

    vs_out.TexCoords = TexCoords;
    vs_out.Normal = normalize(mat3(transpose(inverse(model))) * aNormal);
    vs_out.FragPos = vec3(model * vec4(aPos, 1.0));

    gl_Position = projection * view * model * vec4(aPos, 1.0);
}
