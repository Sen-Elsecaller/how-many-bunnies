shader_type canvas_item;
render_mode unshaded;

// Sombras dinámicas basadas en ray-marching
// Requiere LightOccluder2D en la escena para generar SDF

uniform vec4 shadow_color : source_color = vec4(0.1, 0.1, 0.1, 0.8);
uniform float shadow_angle : hint_range(0, 360) = 120.0;
uniform float shadow_length : hint_range(0, 1000) = 12.0;

void fragment() {
	float angle_rad = shadow_angle * 3.1416 / 180.0;
	vec2 direction = vec2(sin(angle_rad), cos(angle_rad));
	vec2 pos = screen_uv_to_sdf(SCREEN_UV);

	// Ray-marching: simula viaje de luz
	float distance_travelled = 0.0;
	while (distance_travelled < shadow_length) {
		float dist = texture_sdf(pos);
		distance_travelled += dist;
		if (dist < 0.01) {
			break;
		}
		pos += dist * direction;
	}

	// Intensidad de sombra
	float shadow = 1.0 - min(1.0, distance_travelled / shadow_length);
	shadow = ceil(shadow);  // Bordes duros (quitar para sombras suaves)

	COLOR = vec4(shadow_color.rgb, shadow * shadow_color.a);
}
