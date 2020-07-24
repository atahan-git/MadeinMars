------------------------------------------------------------------------------------------------------------------------------------------------------

-- tweak map generator presets

------------------------------------------------------------------------------------------------------------------------------------------------------

-- add tin and gold if preset changes copper or uranium
for _,preset in pairs(data.raw["map-gen-presets"]["default"]) do
	if preset.basic_settings and preset.basic_settings.autoplace_controls then
		preset.basic_settings.autoplace_controls["tin-ore"] = preset.basic_settings.autoplace_controls["copper-ore"]
		preset.basic_settings.autoplace_controls["gold-ore"] = preset.basic_settings.autoplace_controls["uranium-ore"]
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------
