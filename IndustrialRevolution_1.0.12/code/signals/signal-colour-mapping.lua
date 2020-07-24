------------------------------------------------------------------------------------------------------------------------------------------------------

-- lamps

if not mods["Dectorio"] then

	local colours = {
		{"red",1,0,0},
		{"orange",1,0.5,0},
		{"yellow",1,1,0},
		{"green",0,1,0},
		{"cyan",0,1,1},
		{"blue",0,0,1},
		{"pink",1,0,1},
	}

	local map = {}

	for _,colourtable in pairs(colours) do
		table.insert(map, {
		  name = "signal-"..colourtable[1],
		  color = {r=colourtable[2],g=colourtable[3],b=colourtable[4]},
		  type = "virtual"
		})
	end

	for _,entity in pairs(data.raw.lamp) do
		entity.signal_to_color_mapping = map
	end

end

------------------------------------------------------------------------------------------------------------------------------------------------------
