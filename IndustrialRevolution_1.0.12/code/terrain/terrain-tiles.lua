------------------------------------------------------------------------------------------------------------------------------------------------------

local free_layer = 58

function get_next_layer()
	free_layer = free_layer + 2
	return free_layer
end

function get_current_layer()
	return free_layer
end

function generate_gravel_tile(name, mask, layer, colour, speed)
	return {
		name = name,
		type = "tile",
		collision_mask = {"ground-tile"},
		decorative_removal_probability = 1,
		layer = layer,
		map_color = colour,
		minable = {
			mining_time = 0.1,
			result = name,
		},
		mined_sound = {
			filename = "__base__/sound/deconstruct-bricks.ogg"
		},
		needs_correction = false,
		pollution_absorption_per_second = 0,
		transition_overlay_layer_offset = 1,
		transitions = nil,
		variants = {
			main = {
				{
					count = 1,
					picture = "__base__/graphics/terrain/concrete/concrete-dummy.png",
					size = 1,
				},
			},
			material_background = {
				count = 1,
				picture = DIR.terrain_path .. "/" .. name .. ".png",
				scale = 0.5,
			},
			side = {
				count = 1,
				scale = 0.5,
				picture = DIR.terrain_path .. "/" .. name .. "-side.png",			
			},
			inner_corner = {
				count = 1,
				scale = 0.5,
				picture = DIR.terrain_path .. "/" .. name .. "-inner-corner.png",			
			},
			outer_corner = {
				count = 1,
				scale = 0.5,
				picture = DIR.terrain_path .. "/" .. name .. "-outer-corner.png",			
			},
			o_transition = {
				count = 1,
				scale = 0.5,
				picture = DIR.terrain_path .. "/" .. name .. "-o.png",			
			},
			u_transition = {
				count = 1,
				scale = 0.5,
				picture = DIR.terrain_path .. "/" .. name .. "-u.png",			
			},
			side_mask = {
				count = 1,
				scale = 0.5,
				picture = DIR.terrain_path .. "/" .. mask .. "-side-mask.png",			
			},
			inner_corner_mask = {
				count = 1,
				scale = 0.5,
				picture = DIR.terrain_path .. "/" .. mask .. "-inner-corner-mask.png",			
			},
			outer_corner_mask = {
				count = 1,
				scale = 0.5,
				picture = DIR.terrain_path .. "/" .. mask .. "-outer-corner-mask.png",			
			},		
			o_transition_mask = {
				count = 1,
				scale = 0.5,
				picture = DIR.terrain_path .. "/" .. mask .. "-o-mask.png",			
			},
			u_transition_mask = {
				count = 1,
				scale = 0.5,
				picture = DIR.terrain_path .. "/" .. mask .. "-u-mask.png",			
			},
		},
		walking_sound = {
			{
			  filename = DIR.sound_path .. "/gravel-footstep-1.ogg",
			  volume = 0.8
			},
			{
			  filename = DIR.sound_path .. "/gravel-footstep-2.ogg",
			  volume = 0.8
			},
			{
			  filename = DIR.sound_path .. "/gravel-footstep-3.ogg",
			  volume = 0.8
			},
			{
			  filename = DIR.sound_path .. "/gravel-footstep-4.ogg",
			  volume = 0.8
			},
			{
			  filename = DIR.sound_path .. "/gravel-footstep-5.ogg",
			  volume = 0.8
			},
		},
		walking_speed_modifier = speed,
	}
end

for material,speed in pairs(DIR.components.gravel.tile_walking_speed) do
	data:extend({generate_gravel_tile(material.."-gravel", "gravel", get_next_layer(), hsva2rgba(DIR.materials[material].hue, get_saturation(DIR.materials[material].hue,DIR.materials[material].saturation)/2, 0.25, 1), speed)})
end

data.raw.tile["stone-path"].walking_speed_modifier = 1.25

data.raw.tile["stone-path"].layer = get_next_layer()
data.raw.tile["concrete"].layer = get_next_layer()
data.raw.tile["hazard-concrete-left"].layer = get_next_layer()
data.raw.tile["hazard-concrete-right"].layer = get_current_layer()
data.raw.tile["refined-concrete"].layer = get_next_layer()
data.raw.tile["refined-hazard-concrete-left"].layer = get_next_layer()
data.raw.tile["refined-hazard-concrete-right"].layer = get_current_layer()

------------------------------------------------------------------------------------------------------------------------------------------------------
