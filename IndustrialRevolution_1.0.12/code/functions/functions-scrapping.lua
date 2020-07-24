------------------------------------------------------------------------------------------------------------------------------------------------------

local function get_item_count_in_recipe_ingredients(recipe,item)
	local root = recipe.normal or recipe
	if not root.ingredients then return 0 end
	for _,ingredient in pairs(root.ingredients) do
		local name = ingredient.name or ingredient[1]
		local amount = ingredient.amount or ingredient[2]
		if name == item and amount then return amount end
	end
	return 0
end

local function get_catalyst_count_in_recipe_ingredients(recipe,item)
	local root = recipe.normal or recipe
	if not root.ingredients then return 0 end
	for _,ingredient in pairs(root.ingredients) do
		local name = ingredient.name or ingredient[1]
		local amount = ingredient.catalyst_amount
		if name == item and amount then return amount end
	end
	return 0
end

local function get_exclusion_count_in_recipe_ingredients(recipe,item)
	local root = recipe.normal or recipe
	if not root.ingredients then return 0 end
	for _,ingredient in pairs(root.ingredients) do
		local name = ingredient.name or ingredient[1]
		local amount = ingredient.scrapping_exclusion_amount
		if name == item and amount then return amount end
	end
	return 0
end

local function is_recipe_one_result(recipe)
	local root = recipe.normal or recipe
	return root.result or (root.results and (#root.results == 1))
end

local function get_first_result_amount(recipe)
	local root = recipe.normal or recipe
	return (root.result_count or (root.results and (root.results[1].amount or root.results[1][2])) or 1)
end

local function get_first_result_name(recipe)
	local root = recipe.normal or recipe
	return (root.result or (root.results and root.results[1] and (root.results[1].name or root.results[1][1])))
end

local function is_a_crafting_recipe(recipe)
	if (recipe.category == nil) then return true end
	local categories = {"crafting","electroplating","charging"}
	for _,category in pairs(categories) do
		if string.find(recipe.category,category) then return true end
	end
	-- glass is fudged because it bends the basic premise that we're only interested in crafting
	if string.find(recipe.name,"glass") then return true end
	return false
end

local function is_scrapdata_queued(item_data,scrapqueue)
	for _,scrapdata in pairs(scrapqueue) do
		if (scrapdata.item == item_data.item) and (scrapdata.base_item == item_data.base_item) then return true end
	end
	return false
end

function calculate_scraps(item_data, scraptable, count)
	-- check for loop just in case some bonkers situation has arisen with impossible recipe chains
	if not count then count = 1 else count = count + 1 end
	if count > 100 then
		spam_log("Scrap calculator encountered an unexpected cycle in recipe chains, bailing out.", DIR.log_level.error)
		return
	end
	-- search all recipes for this item and build up the scrap table
	local scrapqueue = {}
	for _,recipe in pairs(data.raw.recipe) do
		local name = get_first_result_name(recipe)
		if name and not recipe.hidden and is_a_crafting_recipe(recipe) and is_recipe_one_result(recipe) and recipe.allow_as_intermediate ~= false and recipe.allow_intermediates ~= false and get_first_result_amount(recipe) >= 1 then
			local value = get_item_count_in_recipe_ingredients(recipe,item_data.item) - get_exclusion_count_in_recipe_ingredients(recipe,item_data.item)
			if value > 0 then
				value = (value / get_first_result_amount(recipe)) * item_data.base_value
				if not scraptable[name] then scraptable[name] = {} end
				if not scraptable[name][item_data.base_item] then scraptable[name][item_data.base_item] = 0 end
				scraptable[name][item_data.base_item] = scraptable[name][item_data.base_item] + value
				local scrapdata = {item = name, base_value = value, base_item = item_data.base_item }
				if not is_scrapdata_queued(scrapdata,scrapqueue) then
					table.insert(scrapqueue,scrapdata)
				end
			end
		end
	end
	for _,scrapdata in pairs(scrapqueue) do
		calculate_scraps(scrapdata, scraptable, count)
	end
end

------------------------------------------------------------------------------------------------------------------------------------------------------
