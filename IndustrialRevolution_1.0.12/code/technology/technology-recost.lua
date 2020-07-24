------------------------------------------------------------------------------------------------------------------------------------------------------

-- TECHNOLOGY WORK-OVER PART 3
-- rework costs, based on tree depth

------------------------------------------------------------------------------------------------------------------------------------------------------

-- run an ancestor search - re-cost might be called in data-final-fixes and a cycle may have been created by another mod in the meantime

for _,tech in pairs(data.raw.technology) do
	_ = get_ancestors_of_tech(tech.name)
end

-- work through the science packs - any tech with a science pack tech unlock as an ancestor must use that pack as an ingredient
for id,pack in pairs(DIR.science_packs) do
	if id ~= "automation" then
		local unlocker = data.raw.technology[pack]
		if not unlocker then error("Tried to build tree from non-existent tech pack "..pack) end
		local children = find_techs_with_prereq(pack)
		for _,child in pairs(children) do
			recursive_add_pack_to_tech(pack, child)			
		end
	end
end

-- find all techs with no prereqs at all - that's depth 1

local depth = {}
for _,tech in pairs(data.raw.technology) do
	if tech.enabled ~= false and (tech.prerequisites == nil or #tech.prerequisites == 0) then depth[tech.name] = 1 end
end

-- any techs that have a tech with depth 1 as a prereq are depth 2. rinse and repeat

for d=2,100 do
	local found = false
	for _,tech in pairs(data.raw.technology) do
		if tech.enabled ~= false and tech.prerequisites ~= nil then
			for _,prereq in pairs(tech.prerequisites) do
				if depth[prereq] == d-1 and (not depth[tech.name] or depth[tech.name] < d) then
					depth[tech.name] = d
					found = true
				end
			end
		end
	end
	if not found then break end
end

-- assign costs based on tree depth

local costs = {10,20,30,40,50, 75,100,125,150,175, 200,250,300,350,400, 500,600,700,800,900, 1000,1250,1500,1750,2000}
for tech,depth in pairs(depth) do
	local cost = (depth <= 25) and costs[depth] or (depth-23)*1000
	if data.raw.technology[tech].unit.count then data.raw.technology[tech].unit.count = cost end
	if data.raw.technology[tech].max_level == "infinite" then
		cost = math.ceil(cost/1000)*1000
		local level = string.match(tech, "%d+") or 1
		data.raw.technology[tech].unit.count = nil
		data.raw.technology[tech].unit.count_formula = "2^(L-" .. level .. ")*" .. cost
	end
	data.raw.technology[tech].unit.time = DIR.base_research_time * (DIR.technologies[tech] and DIR.technologies[tech].time_multiplier or 1)
end

------------------------------------------------------------------------------------------------------------------------------------------------------
