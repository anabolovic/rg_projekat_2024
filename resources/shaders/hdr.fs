#version 330 core

out vec4 FragColor;

in vec2 TexCoords;
in vec3 FragPos;
in vec3 Normal;
in vec3 Tangent;
in vec3 Bitangent;

uniform sampler2D diffuseMap;
uniform sampler2D normalMap;
uniform sampler2D heightMap;
uniform vec3 viewPos;
uniform bool parallaxMapping;

uniform sampler2D scene;
uniform sampler2D bloomBlur;
uniform bool bloom;
uniform bool hdr;
uniform float exposure;

const float gamma = 2.2;
const float height_scale = 0.1;

vec2 ParallaxMapping(vec2 texCoords, vec3 viewDir) {
    float height = texture(heightMap, texCoords).r; // sample height map
    vec2 p = viewDir.xy / viewDir.z * (height * height_scale);
    return texCoords - p;
}

void main() {
    vec3 viewDir = normalize(viewPos - FragPos);

    vec2 texCoords = TexCoords;
    if (parallaxMapping) {
        texCoords = ParallaxMapping(TexCoords, viewDir);
    }

    vec3 normal = texture(normalMap, texCoords).rgb;
    normal = normalize(normal * 2.0 - 1.0); // transform normal vector to range [-1, 1]

    vec3 color = texture(diffuseMap, texCoords).rgb;

    // Phong lighting
    vec3 ambient = 0.1 * color;
    vec3 lightDir = normalize(vec3(0.0, 1.0, 1.0)); // fixed light direction for simplicity
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * color;
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0);
    vec3 specular = spec * vec3(1.0);

    vec3 hdrColor = ambient + diffuse + specular;

    // Bloom and HDR
    vec3 bloomColor = texture(bloomBlur, TexCoords).rgb;
    if (bloom) {
        hdrColor += bloomColor;
    }

    vec3 result = hdrColor;
    if (hdr) {
        result = vec3(1.0) - exp(-hdrColor * exposure);
    }

    result = pow(result, vec3(1.0 / gamma));
    FragColor = vec4(result, 1.0);
}
