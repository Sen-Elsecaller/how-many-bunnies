shader_type canvas_item;

// Rayos de luz procedurales animados
// Aplicar a ColorRect que cubra viewport

uniform vec4 color : source_color = vec4(1.0, 1.0, 0.9, 0.3);
uniform float angle = -0.5;
uniform float position = -0.5;
uniform float starting_point = 5.0;
uniform float movement_speed = 1.0;
uniform float ray_separation : hint_range(0.0, 1.0) = 1.0;
uniform float ray_spread_horizontal : hint_range(-1.0, 1.0) = -1.0;
uniform float ray_spread_vertical : hint_range(0.0, 1.0) = 1.0;
uniform float ray_1_density = 20.0;
uniform float ray_2_density = 80.0;
uniform float ray_1_intensity : hint_range(0.0, 1.0) = 0.5;
uniform float ray_2_intensity : hint_range(0.0, 1.0) = 0.5;

// Rotación de rayos
mat2 rotate(float _angle) {
	return mat2(vec2(cos(_angle), -sin(_angle)), vec2(sin(_angle), cos(_angle)));
}

// Ruido pseudo-aleatorio (Perlin simplificado)
float random(vec2 uv) {
	return fract(sin(dot(uv.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

float noise(vec2 uv) {
	vec2 i = floor(uv);
	vec2 f = fract(uv);
	float a = random(i);
	float b = random(i + vec2(1.0, 0.0));
	float c = random(i + vec2(0.0, 1.0));
	float d = random(i + vec2(1.0, 1.0));
	vec2 u = f * f * (3.0 - 2.0 * f);
	return mix(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}

void fragment() {
	// Posicionamiento y rotación
	vec2 ray_pos = (rotate(angle) * (UV - position)) / ((UV.y + ray_separation) - (UV.y * ray_separation));

	// Animación de rayos (dos capas con diferentes densidades)
	vec2 ray_1 = vec2(ray_pos.x * ray_1_density + sin(TIME * 0.1 * movement_speed) * (ray_1_density * 0.2) + starting_point, 1.0);
	vec2 ray_2 = vec2(ray_pos.x * ray_2_density + sin(TIME * 0.2 * movement_speed) * (ray_1_density * 0.2) + starting_point, 1.0);

	// Limitar extensión horizontal
	float bounds = step(ray_spread_horizontal, ray_pos.x) * step(ray_spread_horizontal, 1.0 - ray_pos.x);
	ray_1 *= bounds;
	ray_2 *= bounds;

	// Combinar capas de ruido
	float rays = clamp(noise(ray_1) * ray_1_intensity + noise(ray_2) * ray_2_intensity, 0.0, 1.0);

	// Suavizar bordes
	rays *= smoothstep(0.0, ray_spread_vertical, 1.0 - UV.y);
	rays *= smoothstep(ray_spread_horizontal, ray_spread_horizontal + 0.1, ray_pos.x);
	rays *= smoothstep(ray_spread_horizontal, ray_spread_horizontal + 0.1, 1.0 - ray_pos.x);

	// Aplicar color
	vec3 ray_color = vec3(rays) * color.rgb;
	COLOR = vec4(ray_color, rays * color.a);
}
