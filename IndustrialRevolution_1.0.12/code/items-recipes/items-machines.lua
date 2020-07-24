------------------------------------------------------------------------------------------------------------------------------------------------------

-- main DIR machines

local count = 1
for machine,machinedata in pairs(DIR.machines) do
	local icon = nil
	local icons = nil
	if DIR.icon_masks[machine] then
		icons = {
			{ icon = get_icon_path(DIR.icon_masks[machine].base, DIR.icon_size), icon_size = DIR.icon_size }, 
			{ icon = get_icon_path(DIR.icon_masks[machine].mask, DIR.icon_size), icon_size = DIR.icon_size, tint = DIR.icon_masks[machine].tint }, 
		}
	else
		icon = get_icon_path(machine, DIR.icon_size)
	end
	data:extend({
		{
			type = machinedata.type or "item",
			name = machine,
			order = machinedata.order or string.format("machine-%02d", count),
			subgroup = machinedata.subgroup,
			stack_size = machinedata.stack_size or 50,
			icon = icon,
			icons = icons,
			icon_size = DIR.icon_size,
			place_result = machine,
			ir_machine = true,
		},
		{
			type = "recipe",
			name = machine,
			order = machinedata.order or string.format("machine-%02d", count),
			subgroup = machinedata.subgroup,
			result = machine,
			result_count = 1,
			category = "crafting",
			enabled = machinedata.enabled,
			ingredients = machinedata.ingredients,
			energy_required = machinedata.time or 5,
		}
	})
	count = count + 1
end

------------------------------------------------------------------------------------------------------------------------------------------------------

