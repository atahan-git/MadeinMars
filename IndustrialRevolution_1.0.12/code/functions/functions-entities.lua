------------------------------------------------------------------------------------------------------------------------------------------------------

-- brighter version of tier colour for working vis glow & lights

function brighter_colour(c)
	local w = 240
	return { r = math.floor((c.r + w)/2), g = math.floor((c.g + w)/2), b = math.floor((c.b + w)/2) }
end

-- for calculating scales of energy, health etc.

function get_scale(this_tier, tiers, lowest, highest)
	if this_tier == tiers then return highest end
	return lowest + ((highest - lowest) * ((this_tier - 1) / (tiers - 1)))
end

-- Energy and pollution. They just couldn't make it easy, could they

function get_energy_table(this_tier, tiers, lowest, highest, passive_multiplier)
	local total = get_scale(this_tier, tiers, lowest, highest)
	local passive_energy_usage = total * passive_multiplier
	local active_energy_usage = total * (1 - passive_multiplier)
	return {
		passive = passive_energy_usage .. "KW", -- passive energy drain as a string
		active = active_energy_usage .. "KW", -- active energy usage as a string
	}
end

-- Work out belts-of-coal-per-machine and return as pair of KW strings

function get_scaled_energy_usage(speed, drain_ratio)
	if not drain_ratio then drain_ratio = 0 end
	local total = (DIR.belt_energy_value / (DIR.machines_per_belt * DIR.clusters_per_energy_belt)) * speed * 1000
	return {
		active = tostring(total * (1-drain_ratio)) .. "KW",
		passive = drain_ratio and (tostring(total * (drain_ratio)) .. "KW") or nil,
	}
end

-- shorten vector

function divide_vector(v,f)
	return {v[1]/f, v[2]/f}
end

-- nightmare fuel

function wild_stab_at_entity_by_name(entity_name)
	if not entity_name then return nil end
	-- ugh, like, a world of ugh
	for _,class in pairs(data.raw) do
		-- jesus christ make it stop
		if class ~= "land-mine" then
			for _,candidate in pairs(class) do
				-- max_health isn't mandatory but it's explicitly set for everything we care about
				if candidate.name == entity_name and candidate.selection_box and candidate.max_health then return candidate end
			end
		end
	end
	return nil
end

------------------------------------------------------------------------------------------------------------------------------------------------------

-- sprite definitions for animation layers etc.

function get_sprite_def(name, resolution, frame_count, line_length, shadow, repeat_count, animation_speed, width, height, x, y, scale, shift, blend_mode, flags, tint, direction_count, apply_runtime_tint, run_mode) 
	local variation_count = nil
	if frame_count and frame_count < 0 then
		variation_count = math.abs(frame_count)
		frame_count = nil
	end
	return {
		draw_as_shadow = shadow,
		filename = string.format("%s/%s/%s.png", DIR.sprites_path, resolution, name),
		blend_mode = blend_mode,
		animation_speed = animation_speed,
		repeat_count = repeat_count,
		frame_count = frame_count,
		direction_count = direction_count,
		line_length = line_length,
		height = height,
		width = width,
		x = x,
		y = y,
		scale = scale,
		shift = shift,
		tint = tint,
		apply_runtime_tint = apply_runtime_tint,
		run_mode = run_mode,
		priority = "high",
		flags = flags,
		variation_count = variation_count,
	}
end

function get_multi_sprite_def(name, files, resolution, frame_count, line_length, shadow, repeat_count, animation_speed, width, height, x, y, scale, shift, blend_mode, flags, tint, direction_count, apply_runtime_tint, run_mode) 
	local stripes = {}
	for i=1,files do
		table.insert(stripes, { filename = string.format("%s/%s/%s-%d.png", DIR.sprites_path, resolution, name, i), width_in_frames = frame_count, height_in_frames = direction_count / files })
	end
	return {
		draw_as_shadow = shadow,
		stripes = stripes, 
		blend_mode = blend_mode,
		animation_speed = animation_speed,
		repeat_count = repeat_count,
		frame_count = frame_count,
		direction_count = direction_count,
		line_length = line_length,
		height = height,
		width = width,
		x = x,
		y = y,
		scale = scale,
		shift = shift,
		tint = tint,
		apply_runtime_tint = apply_runtime_tint,
		run_mode = run_mode,
		priority = "high",
		flags = flags,
		max_advance = 1, -- caps vehicle wheel animation
	}
end

function offset(shift1, shift2)
	return ({shift1[1]-shift2[1], shift1[2]-shift2[2]})
end

function shift_calc(x,y,tw,th,w,h)
	return {((tw/2) - (x + (w/2)))/64, ((th/2) - (y + (h/2)))/64}
end

function get_layer(name, frame_count, line_length, shadow, repeat_count, animation_speed, width, height, x, y, tw, th, shift, blend_mode, flags, tint, direction_count, apply_runtime_tint, run_mode, scale) 
	if not scale then scale = 0.5 end
	if shift then shift = offset(shift, shift_calc(x,y,tw,th,width,height)) end
	local layer = get_sprite_def(name, "", frame_count, line_length, shadow, repeat_count, animation_speed, width, height, x, y, scale, shift, blend_mode, flags, tint, direction_count, apply_runtime_tint, run_mode)
	return layer
end

function get_multi_layer(name, files, frame_count, line_length, shadow, repeat_count, animation_speed, width, height, x, y, tw, th, shift, blend_mode, flags, tint, direction_count, apply_runtime_tint, run_mode) 
	shift = offset(shift, shift_calc(x,y,tw,th,width,height))
	local layer = get_multi_sprite_def(name, files, "", frame_count, line_length, shadow, repeat_count, animation_speed, width, height, x, y, 0.5, shift, blend_mode, flags, tint, direction_count, apply_runtime_tint, run_mode)
	return layer
end

------------------------------------------------------------------------------------------------------------------------------------------------------
