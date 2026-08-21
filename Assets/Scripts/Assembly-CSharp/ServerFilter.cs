using System;
using System.Collections.Generic;
using System.Linq;
using GameCore;
using NorthwoodLib;
using UnityEngine;
using UnityEngine.UI;

public class ServerFilter : MonoBehaviour
{
	public InputField NameFilterField;
	public Sprite[] SelectBoxes;
	public Image[] FilterImages;
	public ServerTab CurrentTab;
	public List<ServerListItem> FilteredListItems = new List<ServerListItem>();
	public NewServerBrowser _browser;
	public int ScrollStartPoint = 1;
	public ServerElementButton[] PremadeElements;

	private static FilterSettings _curFilters = new FilterSettings();

	private void Awake()
	{
		_browser = GetComponent<NewServerBrowser>();
		_curFilters = new FilterSettings
		{
			Tab = 0,
			NameFilter = string.Empty,
			Checkboxes = new bool[3]
		};
	}

	public void ChangeTab(int tab)
	{
		if (CurrentTab != (ServerTab)tab)
			ScrollStartPoint = 0;

		_curFilters.Tab = (ServerTab)tab;
		CurrentTab = (ServerTab)tab;
		ReapplyFilters(false);

		if ((ServerTab)tab == ServerTab.Internet)
			_browser?.Refresh();
	}

	public void Filters(int id)
	{
		var image = FilterImages[id];
		_curFilters.Checkboxes[id] = !_curFilters.Checkboxes[id];
		image.sprite = _curFilters.Checkboxes[id] ? SelectBoxes[1] : SelectBoxes[0];
		ReapplyFilters(false);
	}

	public void ApplyNameFilter()
	{
		if (NameFilterField != null)
		{
			string text = NameFilterField.text.ToLowerInvariant();
			_curFilters.NameFilter = text;
			ReapplyFilters(false);
		}
	}

	private void ApplyNameFilter(string nameFilter)
	{
		_curFilters.NameFilter = nameFilter.ToLowerInvariant();
		ReapplyFilters(false);
	}

	public void ReapplyFilters(bool forceCleanup = false)
	{
		FilteredListItems.Clear();
		if (_browser == null) return;

		var servers = NewServerBrowser.Servers;
		if (servers == null || servers.Length == 0)
		{
			DisplayServers();
			return;
		}

		for (int i = 0; i < servers.Length; i++)
		{
			// Set points on the array element BEFORE copying the struct into the filtered list.
			SetNameFilterPoints(i);

			var server = servers[i];
			if (!GameCore.Version.ListedServerCompatibilityCheck(server.version))
				continue;

			if (!CheckCheckboxes(server))
				continue;

			// Tab-specific filtering
			bool keep = false;
			switch (_curFilters.Tab)
			{
				case ServerTab.Internet:
					keep = true;
					break;
				case ServerTab.Favorites:
					keep = FavoriteAndHistory.IsInStorage(FavoriteAndHistory.StorageLocation.Favorites, server.serverId.ToString());
					break;
				case ServerTab.History:
					keep = FavoriteAndHistory.IsInStorage(0, server.serverId.ToString());
					break;
				case ServerTab.Friends:
					keep = SteamLobby.FriendsServer.Contains($"{server.ip}:{server.port}");
					break;
				case ServerTab.Official:
					keep = !server.modded;
					break;
			}

			if (keep)
				FilteredListItems.Add(server);
		}

		if (_curFilters.Tab == ServerTab.History)
		{
			FilteredListItems = FilteredListItems
				.OrderByDescending(item => FavoriteAndHistory.History.IndexOf(item.serverId.ToString()))
				.ToList();
		}

		if (!string.IsNullOrWhiteSpace(_curFilters.NameFilter))
		{
			var displayClass = new { MinDistance = 0 };
			int maxNameLength = _curFilters.NameFilter.Length;
			displayClass = new { MinDistance = Mathf.CeilToInt(maxNameLength * 0.5f) + 1 };

			FilteredListItems = FilteredListItems
				.Where(x => x.NameFilterPoints >= displayClass.MinDistance)
				.OrderByDescending(x => x.NameFilterPoints)
				.ToList();
		}

		if (_browser.LoadingText != null)
		{
			_browser.LoadingText.text = FilteredListItems.Count == 0
				? TranslationReader.Get("MainMenu", 54, "NO_TRANSLATION")
				: string.Empty;
		}

		DisplayServers();
	}

	public void DisplayServers()
	{
		if (PremadeElements == null) return;

		int scrollIndex = ScrollStartPoint;
		int maxDisplay = PremadeElements.Length;

		for (int i = 0; i < maxDisplay; i++)
		{
			int serverIndex = scrollIndex + i;
			if (serverIndex < 0 || serverIndex >= FilteredListItems.Count)
			{
				PremadeElements[i].gameObject.SetActive(false);
				continue;
			}

			var element = PremadeElements[i];
			element.gameObject.SetActive(true);

			var server = FilteredListItems[serverIndex];
			var playButton = element.GetComponent<PlayButton>();
			if (playButton != null)
			{
				playButton.Ip = server.ip;
				playButton.ServerID = server.serverId;
				playButton.Port = server.port.ToString();
				playButton.Motd.text = StringUtils.Base64Decode(server.info);
				playButton.InfoType = Misc.ValidatePastebin(server.pastebin) ? server.pastebin : "7wV681fT";
				playButton.Players.text = server.players;
			}

			if (element.icons.Length >= 4)
			{
				element.icons[0].SetActive(server.officialCode > 0);
				element.icons[1].SetActive(server.whitelist);
				element.icons[2].SetActive(server.friendlyFire);
				element.icons[3].SetActive(server.modded);
			}

			element.SetValues();
		}
		for (int i = maxDisplay; i < PremadeElements.Length; i++)
			PremadeElements[i].gameObject.SetActive(false);
	}

	private static bool CheckCheckboxes(ServerListItem server)
	{
		var checkboxes = _curFilters.Checkboxes;
		if (checkboxes == null || checkboxes.Length < 3)
			return true;

		if (checkboxes[2] && server.whitelist)
			return false;

		if (server.players == null || !server.players.Contains("/"))
			return false;

		string[] parts = server.players.Split('/');
		if (parts.Length < 2 ||
			!int.TryParse(parts[0], out int current) ||
			!int.TryParse(parts[1], out int max))
			return false;

		if (checkboxes[0] && current <= 0)
			return false;

		if (checkboxes[1] && current >= max)
			return false;

		return true;
	}

	private static void SetNameFilterPoints(int id)
	{
		if (string.IsNullOrWhiteSpace(_curFilters.NameFilter))
			return;

		var servers = NewServerBrowser.Servers;
		if (servers == null || id >= servers.Length) return;

		string decodedName = StringUtils.Base64Decode(servers[id].info)
						   ?.ToLowerInvariant();
		if (decodedName == null) return;

		string processed = Misc.ProcessSmallText(decodedName, 35, 75);
		string stripped = StringUtils.StripTags(processed);

		string common = Misc.LongestCommonSubstring(stripped, _curFilters.NameFilter);
		// ServerListItem is a struct — write into the array element, not a copy.
		servers[id].NameFilterPoints = common?.Length ?? 0;
	}

	private class FilterSettings
	{
		public ServerTab Tab;
		public string NameFilter;
		public bool[] Checkboxes;
	}
}