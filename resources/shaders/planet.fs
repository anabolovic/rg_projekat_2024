#version 330 core

struct PointLight {
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct SpotLight {
    vec3 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};


vec3 CalcPointLight(PointLight light, vec3 lightPos, vec3 fragPos, vec3 viewPos, vec3 normal, vec2 texCoords, sampler2D diffuseMap, float shininess);
vec3 CalcSpotLight(SpotLight light, vec3 lightPos, vec3 fragPos, vec3 viewPos, vec3 normal, vec2 texCoords, sampler2D diffuseMap, float shininess);

in VS_OUT {
    vec3 FragPos;
    vec2 TexCoords;
    vec3 Normal;
} fs_in;

layout (location = 0) out vec4 FragColor;
layout (location = 1) out vec4 BrightColor;


uniform PointLight pointLight;
uniform SpotLight spotLight;
uniform sampler2D texture_diffuse1;
uniform vec3 viewPos;

void main()
{
    vec3 result = CalcPointLight(pointLight, pointLight.position, fs_in.FragPos, viewPos, fs_in.Normal, fs_in.TexCoords, texture_diffuse1, 32.0f);
    result += CalcSpotLight(spotLight, spotLight.position, fs_in.FragPos, viewPos, fs_in.Normal, fs_in.TexCoords, texture_diffuse1, 32.0f);
    //    vec3 result = vec3(10.0, 0.0, 10.0);
    FragColor = vec4(result, 1.0);

    float brightness = dot(result, vec3(0.2126, 0.7152, 0.0722));
    if (brightness > 1.0) {
        BrightColor = vec4(result, 1.0f);
    } else {
        BrightColor = vec4(0.0, 0.0, 0.0, 1.0f);
    }
}

// calculates the color when using a point light.
vec3 CalcPointLight(PointLight light, vec3 lightPos, vec3 fragPos, vec3 viewPos, vec3 normal, vec2 texCoords, sampler2D diffuseMap, float shininess)
{
    vec3 lightDir = normalize(lightPos - fragPos);
    //    vec3 reflectDir = reflect(-lightDir, normal);
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 halfwayDir = normalize(lightDir + viewDir);
    float distance = length(lightPos - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

    // diffuse factor
    float diff = max(dot(normal, lightDir), 0.0);
    // Specular factor
    float spec = pow(max(dot(normal, halfwayDir), 0.0), shininess);

    // Ambient component
    vec3 ambient = light.ambient * vec3(texture(diffuseMap, texCoords).rgb);

    // Diffuse Component
    vec3 diffuse = light.diffuse * diff * vec3(texture(diffuseMap, texCoords).rgb);

    // Specular Component
    vec3 specular = light.specular * spec;
    //    if (specularMap >= 0) {
    //        specular *= vec3(texture(specularMap, TexCoords).xxx);
    //    }

    // Attenuation
    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;

    return (ambient + diffuse + specular);
}

vec3 CalcSpotLight(SpotLight light, vec3 lightPos, vec3 fragPos, vec3 viewPos, vec3 normal, vec2 texCoords, sampler2D diffuseMap, float shininess)
{
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 lightDir = normalize(lightPos - fragPos);
    vec3 halfwayDir = normalize(lightDir + viewDir);

    float theta = dot(lightDir, normalize(-light.direction));
    float epsilon = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

    float distance = length(lightPos - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    float spec = pow(max(dot(normal, halfwayDir), 0.0), shininess);
    // combine results
    vec3 ambient = light.ambient * vec3(texture(diffuseMap, texCoords).rgb);
    vec3 diffuse = light.diffuse * diff * vec3(texture(diffuseMap, texCoords).rgb);
    vec3 specular = light.specular * spec;
    ambient *= attenuation * intensity;
    diffuse *= attenuation * intensity;
    specular *= attenuation * intensity;
    return (ambient + diffuse + specular);
}