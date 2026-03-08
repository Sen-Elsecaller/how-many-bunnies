class_name AnimatedButton extends Button

var tween: Tween

func _ready():
	focus_mode = Control.FOCUS_NONE 
	resized.connect(_update_pivot)
	_update_pivot()
	
	button_down.connect(_on_button_down)
	button_up.connect(_on_button_up)
	mouse_entered.connect(_on_focus_entered)
	mouse_exited.connect(_on_focus_exited)

func _update_pivot():
	pivot_offset = size * Vector2(0.5, 1.0)

func _load_styles():
	if Database.styles.has("animated_button"):
		for style_name in Database.styles["animated_button"]:
			var style_resource = Database.styles["animated_button"][style_name]
			add_theme_stylebox_override(style_name, style_resource)
	
func _on_focus_entered():
	if tween: tween.kill()
	scale = Vector2(1.04, 1.04)

func _on_focus_exited():
	if tween: tween.kill()
	scale = Vector2(1, 1)
	
func _on_button_down():
	if tween: tween.kill()
	scale = Vector2(0.95, 0.875)

func _on_button_up():
	tween = create_tween().set_ease(Tween.EASE_OUT)
	tween.set_trans(Tween.TRANS_ELASTIC)
	tween.tween_property(self, "scale", Vector2(1, 1), 0.25)
	
