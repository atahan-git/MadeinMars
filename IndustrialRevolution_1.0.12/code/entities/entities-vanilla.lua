------------------------------------------------------------------------------------------------------------------------------------------------------

-- MISC TWEAKS TO VANILLA ENTITIES

------------------------------------------------------------------------------------------------------------------------------------------------------

-- gui keys

data.raw["assembling-machine"]["oil-refinery"].gui_title_key = "gui-title.refinery"
data.raw["assembling-machine"]["chemical-plant"].gui_title_key = "gui-title.chemistry"

-- inventories buff

data.raw.container["wooden-chest"].inventory_size = 20
data.raw.container["iron-chest"].inventory_size = 60 -- bronze chest is 40
data.raw.container["steel-chest"].inventory_size = 80
data.raw["logistic-container"]["logistic-chest-active-provider"].inventory_size = 80
data.raw["logistic-container"]["logistic-chest-buffer"].inventory_size = 80
data.raw["logistic-container"]["logistic-chest-passive-provider"].inventory_size = 80
data.raw["logistic-container"]["logistic-chest-requester"].inventory_size = 80
data.raw["logistic-container"]["logistic-chest-storage"].inventory_size = 80
data.raw["cargo-wagon"]["cargo-wagon"].inventory_size = 80

-- wire reach buff

data.raw["electric-pole"]["big-electric-pole"].maximum_wire_distance = 32

-- rocket silo inventory

data.raw["rocket-silo"]["rocket-silo"].rocket_result_inventory_size = 10

-- oil refinery lockdown

data.raw["assembling-machine"]["oil-refinery"].fixed_recipe = "basic-oil-processing"

-- accumulator buff

data.raw.accumulator["accumulator"].energy_source = {
    buffer_capacity = "60MJ",
    input_flow_limit = "1MW",
    output_flow_limit = "1MW",
    type = "electric",
    usage_priority = "tertiary"
}
data.raw.accumulator["accumulator"].max_health = 300

-- banish unwanted things

data.raw.furnace["stone-furnace"].flags = {"hidden"}
data.raw.furnace["steel-furnace"].flags = {"hidden"}
data.raw.furnace["electric-furnace"].flags = {"hidden"}

-- oil refinery working vis & adjust oil processing energy costs

if data.raw["assembling-machine"]["oil-refinery"] and data.raw["assembling-machine"]["oil-refinery"].working_visualisations[1] then
	data.raw["assembling-machine"]["oil-refinery"].working_visualisations[1].light.color = DIR.oil_flame_colour
	data.raw["assembling-machine"]["oil-refinery"].working_visualisations[1].light.intensity = 0.75
	data.raw["assembling-machine"]["oil-refinery"].energy_usage = get_scaled_energy_usage(2, 0.04).active
	data.raw["assembling-machine"]["oil-refinery"].energy_source.drain = get_scaled_energy_usage(2, 0.04).passive
end
if data.raw["assembling-machine"]["chemical-plant"] then
	data.raw["assembling-machine"]["chemical-plant"].energy_usage = get_scaled_energy_usage(1, 0.04).active
	data.raw["assembling-machine"]["chemical-plant"].energy_source.drain = get_scaled_energy_usage(1, 0.04).passive
end

-- default upgrades for vanilla small chests

data.raw.container["wooden-chest"].next_upgrade = "bronze-chest"
data.raw.container["iron-chest"].next_upgrade = "steel-chest"

-- night glow for chem plants

if data.raw["assembling-machine"]["chemical-plant"] and data.raw["assembling-machine"]["chemical-plant"].working_visualisations then
	table.insert(data.raw["assembling-machine"]["chemical-plant"].working_visualisations, {
        light = { color = {1,1,1}, intensity = 0.65, size = 9},
        apply_recipe_tint = "quaternary",		
	})
end

-- Put a warning on storage tanks

data.raw["storage-tank"]["storage-tank"].localised_description = {"entity-description.storage-tank-pollution"}

------------------------------------------------------------------------------------------------------------------------------------------------------