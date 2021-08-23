require("Other/UIManager")
require("Other/GlobalVariable")
require("Other/DoTweenManager")

UIStart = {}

UIStart.Start = function()

	print("UIStart")

	--CS.LoadSceneManager.Instance:LoadSceneAsync("Level1",function ( ... )
		-- body
		--CS.TimeManager.Instance.UpdatePerSecondEventHandler += SaveDataToLocal;
		UIManager:ShowPanel("MainMenuPanel");
	--end);
--[[
	UIManager:ShowPanel("BGPanel");

	local loginPanel = UIManager:ShowPanel("LoginPanel");
	require("Controller/LoginController");
	LoginController:Start(loginPanel);
]]--
end
