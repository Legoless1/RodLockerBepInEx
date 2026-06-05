using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace RodLockerBepInEx
{
	[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
	public sealed class Plugin : BaseUnityPlugin
	{
		public const string PluginGuid = "legoless.RodLocker";
		public const string PluginName = "Rod Locker";
		public const string PluginVersion = "1.0.0";

		private Harmony harmony;

		internal static ManualLogSource Log { get; private set; }

		private void Awake()
		{
			Log = Logger;
			RodLockerRuntime.PluginAssembly = Assembly.GetExecutingAssembly();

			harmony = new Harmony(PluginGuid);
			harmony.PatchAll();

			Logger.LogInfo("Rod Locker BepInEx Edition loaded. No game files are modified by this plugin DLL.");
		}

		private void OnDestroy()
		{
			harmony?.UnpatchSelf();
		}
	}

	internal static class RodLockerRuntime
	{
		private const string BlackstoneDockId = "dock.outcast-isle";
		private const string RodLockerDestinationId = "destination.rod-locker";
		private const string RodLockerTitleKey = "destination.rod-locker";
		private const GridKey RodLockerGridKey = (GridKey)459001;

		private static readonly FieldInfo DestinationButtonPrefabField = AccessTools.Field(typeof(DockUI), "destinationButtonPrefab");
		private static readonly FieldInfo DestinationButtonContainerField = AccessTools.Field(typeof(DockUI), "destinationButtonContainer");
		private static readonly FieldInfo DestinationButtonObjectsField = AccessTools.Field(typeof(DockUI), "destinationButtonObjects");
		private static readonly FieldInfo DestinationButtonsField = AccessTools.Field(typeof(DockUI), "destinationButtons");
		private static readonly FieldInfo DockDestinationsField = AccessTools.Field(typeof(Dock), "destinations");
		private static readonly FieldInfo BaseDestinationIdField = AccessTools.Field(typeof(BaseDestination), "id");
		private static readonly FieldInfo BaseDestinationTitleKeyField = AccessTools.Field(typeof(BaseDestination), "titleKey");
		private static readonly FieldInfo BaseDestinationSpeakerRootNodeOverrideField = AccessTools.Field(typeof(BaseDestination), "speakerRootNodeOverride");
		private static readonly FieldInfo BaseDestinationAlwaysShowField = AccessTools.Field(typeof(BaseDestination), "alwaysShow");
		private static readonly FieldInfo BaseDestinationIsIndoorsField = AccessTools.Field(typeof(BaseDestination), "isIndoors");
		private static readonly FieldInfo BaseDestinationIconField = AccessTools.Field(typeof(BaseDestination), "icon");
		private static readonly FieldInfo BaseDestinationLoopSfxField = AccessTools.Field(typeof(BaseDestination), "loopSFX");
		private static readonly FieldInfo BaseDestinationVisitSfxField = AccessTools.Field(typeof(BaseDestination), "visitSFX");
		private static readonly FieldInfo BaseDestinationVCamField = AccessTools.Field(typeof(BaseDestination), "vCam");
		private static readonly FieldInfo BaseDestinationHighlightConditionsField = AccessTools.Field(typeof(BaseDestination), "highlightConditions");
		private static readonly FieldInfo BaseDestinationPlayerInventoryTabIndexesField = AccessTools.Field(typeof(BaseDestination), "playerInventoryTabIndexesToShow");
		private static readonly FieldInfo MarketDestinationItemTypesBoughtField = AccessTools.Field(typeof(MarketDestination), "itemTypesBought");
		private static readonly FieldInfo MarketDestinationItemSubtypesBoughtField = AccessTools.Field(typeof(MarketDestination), "itemSubtypesBought");
		private static readonly FieldInfo MarketDestinationBulkItemTypesBoughtField = AccessTools.Field(typeof(MarketDestination), "bulkItemTypesBought");
		private static readonly FieldInfo MarketDestinationBulkItemSubtypesBoughtField = AccessTools.Field(typeof(MarketDestination), "bulkItemSubtypesBought");
		private static readonly FieldInfo MarketDestinationSpecificItemsBoughtField = AccessTools.Field(typeof(MarketDestination), "specificItemsBought");
		private static readonly FieldInfo MarketDestinationSellValueModifierField = AccessTools.Field(typeof(MarketDestination), "sellValueModifier");
		private static readonly FieldInfo MarketDestinationAllowSellIfGridFullField = AccessTools.Field(typeof(MarketDestination), "allowSellIfGridFull");
		private static readonly FieldInfo MarketDestinationAllowStorageAccessField = AccessTools.Field(typeof(MarketDestination), "allowStorageAccess");
		private static readonly FieldInfo MarketDestinationAllowRepairsField = AccessTools.Field(typeof(MarketDestination), "allowRepairs");
		private static readonly FieldInfo MarketDestinationAllowBulkSellField = AccessTools.Field(typeof(MarketDestination), "allowBulkSell");
		private static readonly FieldInfo MarketDestinationBulkSellPromptStringField = AccessTools.Field(typeof(MarketDestination), "bulkSellPromptString");
		private static readonly FieldInfo MarketDestinationBulkSellNotificationStringField = AccessTools.Field(typeof(MarketDestination), "bulkSellNotificationString");
		private static readonly FieldInfo DestinationButtonTextField = AccessTools.Field(typeof(DestinationButton), "textField");
		private static readonly FieldInfo MarketTitleStringField = AccessTools.Field(typeof(MarketDestinationUI), "localizedTitleString");
		private static readonly FieldInfo BaseDestinationUIDestinationField = AccessTools.Field(typeof(BaseDestinationUI), "destination");
		private static readonly FieldInfo ItemManagerAllItemsField = AccessTools.Field(typeof(ItemManager), "allItems");
		private static readonly FieldInfo PlatformSpriteOverridesField = AccessTools.Field(typeof(SpatialItemData), "platformSpecificSpriteOverrides");
		private static readonly FieldInfo ItemOwnPrerequisitesField = AccessTools.Field(typeof(SpatialItemData), "itemOwnPrerequisites");
		private static readonly FieldInfo ResearchPrerequisitesField = AccessTools.Field(typeof(SpatialItemData), "researchPrerequisites");
		private static readonly FieldInfo ResearchPointsRequiredField = AccessTools.Field(typeof(SpatialItemData), "researchPointsRequired");
		private static readonly FieldInfo BuyableWithoutResearchField = AccessTools.Field(typeof(SpatialItemData), "buyableWithoutResearch");
		private static readonly FieldInfo ResearchIsForRecipeField = AccessTools.Field(typeof(SpatialItemData), "researchIsForRecipe");
		private static readonly FieldInfo GridMainItemTypeField = AccessTools.Field(typeof(GridConfiguration), "mainItemType");
		private static readonly FieldInfo GridMainItemSubtypeField = AccessTools.Field(typeof(GridConfiguration), "mainItemSubtype");
		private static readonly FieldInfo GridItemsBelongToPlayerField = AccessTools.Field(typeof(GridConfiguration), "itemsInThisBelongToPlayer");
		private static readonly FieldInfo GridCanAddItemsInQuestModeField = AccessTools.Field(typeof(GridConfiguration), "canAddItemsInQuestMode");
		private static readonly FieldInfo GridHasUnderlayField = AccessTools.Field(typeof(GridConfiguration), "hasUnderlay");

		private static readonly string[] RodItemIds =
		{
			"legoless.rodlocker.playstation",
			"legoless.rodlocker.xbox",
			"legoless.rodlocker.switch",
			"legoless.rodlocker.steam",
			"legoless.rodlocker.gog",
			"legoless.rodlocker.ios",
			"legoless.rodlocker.android"
		};

		private static readonly Dictionary<string, string> RodSpriteResourceNames = new Dictionary<string, string>(StringComparer.Ordinal)
		{
			{ "legoless.rodlocker.playstation", "RodLockerBepInEx.Assets.legoless.rodlocker.playstation.png" },
			{ "legoless.rodlocker.xbox", "RodLockerBepInEx.Assets.legoless.rodlocker.xbox.png" },
			{ "legoless.rodlocker.switch", "RodLockerBepInEx.Assets.legoless.rodlocker.switch.png" },
			{ "legoless.rodlocker.steam", "RodLockerBepInEx.Assets.legoless.rodlocker.steam.png" },
			{ "legoless.rodlocker.gog", "RodLockerBepInEx.Assets.legoless.rodlocker.gog.png" },
			{ "legoless.rodlocker.ios", "RodLockerBepInEx.Assets.legoless.rodlocker.ios.png" },
			{ "legoless.rodlocker.android", "RodLockerBepInEx.Assets.legoless.rodlocker.android.png" }
		};

		private static readonly Dictionary<string, string> TitleTranslations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{ "en", "Rod Locker" },
			{ "fr", "Casier \u00e0 cannes" },
			{ "de", "Angelrutenschrank" },
			{ "it", "Armadietto portacanne" },
			{ "es", "Taquilla de ca\u00f1as" },
			{ "pl", "Szafka na w\u0119dki" },
			{ "ru", "\u0428\u043a\u0430\u0444\u0447\u0438\u043a \u0434\u043b\u044f \u0443\u0434\u043e\u0447\u0435\u043a" },
			{ "pt-BR", "Arm\u00e1rio de varas" },
			{ "ja-JP", "\u30ed\u30c3\u30c9\u30ed\u30c3\u30ab\u30fc" },
			{ "ko-KR", "\ub099\uc2ef\ub300 \ubcf4\uad00\ud568" },
			{ "zh-Hans", "\u9c7c\u7aff\u67dc" },
			{ "zh-Hant", "\u91e3\u7aff\u6ac3" }
		};

		private static bool loggedInjection;
		private static bool registeredItems;
		private static bool loadedEmbeddedSprites;
		private static MarketDestination rodLockerDestination;
		private static GridConfiguration rodLockerGridConfig;
		private static Sprite rodIconSprite;
		private static readonly Dictionary<string, Sprite> RodSprites = new Dictionary<string, Sprite>(StringComparer.Ordinal);

		internal static Assembly PluginAssembly { get; set; }

		internal static BaseDestination EnsureRodLockerDestination(Dock dock)
		{
			if (dock == null || dock.Data == null || dock.Data.Id != BlackstoneDockId)
			{
				return null;
			}

			EnsureRuntimeData();
			EnsureRodLockerGrid();

			if (rodLockerDestination != null)
			{
				return rodLockerDestination;
			}

			try
			{
				var dockDestinations = GetDockDestinations(dock);
				var workshop = dockDestinations?.FirstOrDefault(destination =>
					destination != null &&
					(destination.Id == "destination.outcast-yard" || destination.gameObject.name == "Workshop"));
				var templateMarket = workshop as MarketDestination;
				var destinationParent = workshop != null ? workshop.transform.parent : dock.transform;

				var lockerObject = new GameObject("Rod Locker Destination");
				lockerObject.transform.SetParent(destinationParent, false);
				lockerObject.transform.localPosition = workshop != null
					? workshop.transform.localPosition + new Vector3(0f, 1.25f, 0f)
					: Vector3.zero;

				var destination = lockerObject.AddComponent<MarketDestination>();
				BaseDestinationIdField.SetValue(destination, RodLockerDestinationId);
				BaseDestinationTitleKeyField.SetValue(destination, CreateLocalizedString(RodLockerTitleKey));
				BaseDestinationSpeakerRootNodeOverrideField.SetValue(destination, string.Empty);
				BaseDestinationAlwaysShowField.SetValue(destination, true);
				BaseDestinationIsIndoorsField.SetValue(destination, true);
				BaseDestinationIconField.SetValue(destination, rodIconSprite != null ? rodIconSprite : workshop != null ? workshop.Icon : null);
				BaseDestinationLoopSfxField.SetValue(destination, templateMarket != null ? BaseDestinationLoopSfxField.GetValue(templateMarket) : null);
				BaseDestinationVisitSfxField.SetValue(destination, templateMarket != null ? BaseDestinationVisitSfxField.GetValue(templateMarket) : null);
				BaseDestinationVCamField.SetValue(destination, workshop != null ? workshop.VCam : null);
				BaseDestinationHighlightConditionsField.SetValue(destination, new List<HighlightCondition>());
				destination.selectOnLeft = new List<BaseDestination>();
				destination.selectOnRight = new List<BaseDestination>();
				destination.selectOnUp = new List<BaseDestination>();
				destination.selectOnDown = workshop != null ? new List<BaseDestination> { workshop } : new List<BaseDestination>();
				var playerInventoryTabIndexes = templateMarket != null
					? new List<int>((List<int>)BaseDestinationPlayerInventoryTabIndexesField.GetValue(templateMarket))
					: new List<int> { 0, 1, 2 };
				BaseDestinationPlayerInventoryTabIndexesField.SetValue(destination, playerInventoryTabIndexes);
				MarketDestinationItemTypesBoughtField.SetValue(destination, ItemType.EQUIPMENT);
				MarketDestinationItemSubtypesBoughtField.SetValue(destination, ItemSubtype.ROD);
				MarketDestinationBulkItemTypesBoughtField.SetValue(destination, ItemType.NONE);
				MarketDestinationBulkItemSubtypesBoughtField.SetValue(destination, ItemSubtype.NONE);
				MarketDestinationSpecificItemsBoughtField.SetValue(destination, Array.Empty<SpatialItemData>());
				MarketDestinationSellValueModifierField.SetValue(destination, 0f);
				MarketDestinationAllowSellIfGridFullField.SetValue(destination, false);
				MarketDestinationAllowStorageAccessField.SetValue(destination, true);
				MarketDestinationAllowRepairsField.SetValue(destination, false);
				MarketDestinationAllowBulkSellField.SetValue(destination, false);
				MarketDestinationBulkSellPromptStringField.SetValue(destination, string.Empty);
				MarketDestinationBulkSellNotificationStringField.SetValue(destination, string.Empty);
				destination.marketTabs = new List<MarketTabConfig>
				{
					new MarketTabConfig
					{
						gridKey = RodLockerGridKey,
						tabSprite = rodIconSprite != null ? rodIconSprite : workshop != null ? workshop.Icon : null,
						titleKey = CreateLocalizedString(RodLockerTitleKey),
						isUnlockedBasedOnDialogue = false,
						unlockDialogueNodes = new List<string>()
					}
				};

				rodLockerDestination = destination;
				return destination;
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError($"Failed to create Rod Locker destination: {ex}");
				return null;
			}
		}

		internal static void AddRodLockerButton(DockUI dockUi, Dock dock)
		{
			if (dockUi == null || dock == null || dock.Data == null || dock.Data.Id != BlackstoneDockId)
			{
				return;
			}

			var destination = EnsureRodLockerDestination(dock);
			if (destination == null)
			{
				return;
			}

			try
			{
				var destinationButtons = DestinationButtonsField.GetValue(dockUi) as List<DestinationButton>;
				if (destinationButtons != null && destinationButtons.Any(button => button != null && button.destination != null && button.destination.Id == RodLockerDestinationId))
				{
					return;
				}

				var prefab = DestinationButtonPrefabField.GetValue(dockUi) as GameObject;
				var container = DestinationButtonContainerField.GetValue(dockUi) as Transform;
				var destinationButtonObjects = DestinationButtonObjectsField.GetValue(dockUi) as List<GameObject>;
				if (prefab == null || container == null)
				{
					Plugin.Log.LogError("Rod Locker could not find the vanilla destination button prefab/container.");
					return;
				}

				var buttonObject = UnityEngine.Object.Instantiate(prefab, container);
				var destinationButton = buttonObject.GetComponent<DestinationButton>();
				if (destinationButton == null)
				{
					UnityEngine.Object.Destroy(buttonObject);
					Plugin.Log.LogError("Rod Locker destination button prefab did not contain DestinationButton.");
					return;
				}

				destinationButton.Init(destination);
				SetDestinationButtonText(destinationButton);
				destinationButtons?.Add(destinationButton);
				destinationButtonObjects?.Add(buttonObject);

				if (!loggedInjection)
				{
					Plugin.Log.LogInfo($"Rod Locker UI button added to Blackstone. StoredDestinations={GetDockDestinations(dock)?.Count ?? 0}, BoatActionsReady={dock.boatActionsDestination != null}");
					loggedInjection = true;
				}
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError($"Failed to add Rod Locker UI button: {ex}");
			}
		}

		internal static void ResetCachedDestination()
		{
			rodLockerDestination = null;
			loggedInjection = false;
		}

		internal static void EnsureRuntimeData()
		{
			LoadEmbeddedSprites();
			RegisterRodItems(GameManager.Instance?.ItemManager);
			EnsureRodLockerGridConfig();
		}

		internal static void RegisterRodItems(ItemManager itemManager)
		{
			LoadEmbeddedSprites();

			if (itemManager == null || !itemManager.HasLoaded())
			{
				return;
			}

			var allItems = ItemManagerAllItemsField.GetValue(itemManager) as List<ItemData>;
			if (allItems == null)
			{
				Plugin.Log.LogWarning("Could not access ItemManager item list for Rod Locker registration.");
				return;
			}

			if (registeredItems && HasAllRodItems(allItems))
			{
				return;
			}

			var template = itemManager.GetItemDataById<RodItemData>("rod21");
			if (template == null)
			{
				Plugin.Log.LogWarning("Could not find vanilla rod21 Custom Rod data.");
				return;
			}

			rodIconSprite = template.itemTypeIcon != null ? template.itemTypeIcon : template.GetSprite();

			var definitions = RodItemIds
				.Select(itemId => new RodDefinition(itemId, GetRodSprite(itemId)))
				.ToArray();

			var addedCount = 0;
			foreach (var definition in definitions)
			{
				if (definition.Sprite == null)
				{
					Plugin.Log.LogWarning($"Skipping {definition.Id}: no sprite was available.");
					continue;
				}

				if (allItems.Any(item => item != null && item.id == definition.Id))
				{
					continue;
				}

				var rod = UnityEngine.Object.Instantiate(template);
				rod.name = definition.Id;
				rod.id = definition.Id;
				rod.sprite = definition.Sprite;
				rod.itemColor = new Color32(49, 49, 49, 255);
				rod.value = 0m;
				rod.sellOverrideValue = 0m;
				rod.fishingSpeedModifier = 0.4f;
				rod.harvestableTypes = new[] { HarvestableType.COASTAL };
				rod.aberrationBonus = 0f;
				rod.itemType = ItemType.EQUIPMENT;
				rod.itemSubtype = ItemSubtype.ROD;
				rod.damageMode = DamageMode.OPERATION;
				rod.moveMode = MoveMode.INSTALL;
				rod.entitlementsRequired = new List<Entitlement>();
				PlatformSpriteOverridesField.SetValue(rod, new Dictionary<Platform, Sprite>());
				ItemOwnPrerequisitesField.SetValue(rod, new List<OwnedItemResearchablePrerequisite>());
				ResearchPrerequisitesField.SetValue(rod, new List<ResearchedItemResearchablePrerequisite>());
				ResearchPointsRequiredField.SetValue(rod, 0);
				BuyableWithoutResearchField.SetValue(rod, false);
				ResearchIsForRecipeField.SetValue(rod, false);

				allItems.Add(rod);
				addedCount++;
			}

			registeredItems = HasAllRodItems(allItems);
			Plugin.Log.LogInfo($"Rod Locker registered custom rod item data. Added={addedCount}, Present={CountRodItems(allItems)}/{RodItemIds.Length}");
		}

		internal static void EnsureRodLockerGrid()
		{
			EnsureRuntimeData();

			var saveData = GameManager.Instance?.SaveData;
			if (saveData == null || rodLockerGridConfig == null)
			{
				return;
			}

			if (saveData.grids == null)
			{
				saveData.grids = new Dictionary<GridKey, SerializableGrid>();
			}

			if (!saveData.grids.TryGetValue(RodLockerGridKey, out var grid) || grid == null)
			{
				grid = new SerializableGrid();
				saveData.grids[RodLockerGridKey] = grid;
				grid.Init(rodLockerGridConfig, false);
			}
			else if (grid.GridConfiguration == null)
			{
				grid.Init(rodLockerGridConfig, false);
			}

			RemoveAlreadyTakenStock(grid);
			AddMissingStock(grid);
		}

		internal static bool IsRodLockerDestination(BaseDestination destination)
		{
			return destination != null && destination.Id == RodLockerDestinationId;
		}

		internal static bool IsRodLockerGrid(GridKey gridKey)
		{
			return gridKey == RodLockerGridKey;
		}

		internal static void SetDestinationButtonText(DestinationButton button)
		{
			if (button == null || !IsRodLockerDestination(button.destination))
			{
				return;
			}

			var text = DestinationButtonTextField.GetValue(button) as TextMeshProUGUI;
			if (text != null)
			{
				text.text = GetTranslatedTitle();
			}
		}

		internal static void SetMarketTitle(MarketDestinationUI ui)
		{
			if (ui == null)
			{
				return;
			}

			if (!IsRodLockerMarketUI(ui))
			{
				return;
			}

			var localizeString = MarketTitleStringField.GetValue(ui) as LocalizeStringEvent;
			var text = localizeString != null ? localizeString.GetComponent<TextMeshProUGUI>() : null;
			if (text != null)
			{
				text.text = GetTranslatedTitle();
			}
		}

		internal static bool IsRodLockerMarketUI(MarketDestinationUI ui)
		{
			if (ui == null)
			{
				return false;
			}

			var destination = BaseDestinationUIDestinationField?.GetValue(ui) as BaseDestination;
			return IsRodLockerDestination(destination);
		}

		internal static string GetTranslatedTitle()
		{
			var code = "en";
			try
			{
				code = GameManager.Instance?.LanguageManager?.GetLocale()?.Identifier.Code ?? code;
			}
			catch
			{
				// Keep English fallback during early startup.
			}

			if (TitleTranslations.TryGetValue(code, out var title))
			{
				return title;
			}

			var neutralCode = code.Split('-')[0];
			return TitleTranslations.TryGetValue(neutralCode, out title) ? title : TitleTranslations["en"];
		}

		private static void EnsureRodLockerGridConfig()
		{
			if (rodLockerGridConfig != null)
			{
				return;
			}

			var gameConfig = GameManager.Instance?.GameConfigData;
			var template = gameConfig != null ? gameConfig.GetGridConfigForKey(GridKey.SHIPWRIGHT_RODS) : null;
			rodLockerGridConfig = template != null
				? UnityEngine.Object.Instantiate(template)
				: ScriptableObject.CreateInstance<GridConfiguration>();
			rodLockerGridConfig.name = "legoless.rodlocker.grid";
			rodLockerGridConfig.columns = 7;
			rodLockerGridConfig.rows = 2;
			rodLockerGridConfig.cellGroupConfigs = new List<CellGroupConfiguration>();
			GridMainItemTypeField.SetValue(rodLockerGridConfig, ItemType.EQUIPMENT);
			GridMainItemSubtypeField.SetValue(rodLockerGridConfig, ItemSubtype.ROD);
			GridItemsBelongToPlayerField.SetValue(rodLockerGridConfig, false);
			GridCanAddItemsInQuestModeField.SetValue(rodLockerGridConfig, false);
			GridHasUnderlayField.SetValue(rodLockerGridConfig, false);
		}

		private static List<BaseDestination> GetDockDestinations(Dock dock)
		{
			return dock != null ? DockDestinationsField.GetValue(dock) as List<BaseDestination> : null;
		}

		private static bool HasAllRodItems(List<ItemData> allItems)
		{
			return CountRodItems(allItems) == RodItemIds.Length;
		}

		private static int CountRodItems(List<ItemData> allItems)
		{
			if (allItems == null)
			{
				return 0;
			}

			return RodItemIds.Count(itemId => allItems.Any(item => item != null && item.id == itemId));
		}

		private static void RemoveAlreadyTakenStock(SerializableGrid grid)
		{
			var toRemove = grid.spatialItems
				.Where(item => item != null && RodItemIds.Contains(item.id) && HasRodLockerItemBeenTaken(item.id))
				.ToList();

			foreach (var item in toRemove)
			{
				grid.RemoveObjectFromGridData(item, false);
			}
		}

		private static void AddMissingStock(SerializableGrid grid)
		{
			var itemManager = GameManager.Instance?.ItemManager;
			if (itemManager == null)
			{
				return;
			}

			var existingIds = new HashSet<string>(grid.spatialItems.Where(item => item != null).Select(item => item.id));
			foreach (var itemId in RodItemIds)
			{
				if (existingIds.Contains(itemId) || HasRodLockerItemBeenTaken(itemId))
				{
					continue;
				}

				var itemData = itemManager.GetItemDataById<SpatialItemData>(itemId);
				if (itemData == null)
				{
					Plugin.Log.LogWarning($"Could not stock {itemId}: item data is not registered.");
					continue;
				}

				if (grid.FindSpaceAndAddObjectToGridData(itemData, false, null))
				{
					existingIds.Add(itemId);
				}
				else
				{
					Plugin.Log.LogWarning($"Could not place {itemId} in the Rod Locker grid.");
				}
			}
		}

		private static bool HasRodLockerItemBeenTaken(string itemId)
		{
			var saveData = GameManager.Instance?.SaveData;
			return saveData?.itemTransactions?.Any(transaction =>
				transaction != null &&
				transaction.itemId == itemId &&
				transaction.bought > 0) ?? false;
		}

		private static LocalizedString CreateLocalizedString(string key)
		{
			return new LocalizedString(LanguageManager.STRING_TABLE, key);
		}

		private static Sprite GetRodSprite(string itemId)
		{
			LoadEmbeddedSprites();
			return RodSprites.TryGetValue(itemId, out var sprite) ? sprite : null;
		}

		private static void LoadEmbeddedSprites()
		{
			if (loadedEmbeddedSprites)
			{
				return;
			}

			foreach (var pair in RodSpriteResourceNames)
			{
				var sprite = LoadEmbeddedSprite(pair.Value, pair.Key + ".sprite");
				if (sprite != null)
				{
					RodSprites[pair.Key] = sprite;
				}
			}

			loadedEmbeddedSprites = true;
			Plugin.Log.LogInfo($"Rod Locker loaded {RodSprites.Count}/{RodSpriteResourceNames.Count} embedded rod sprites.");
		}

		private static Sprite LoadEmbeddedSprite(string resourceName, string spriteName)
		{
			using (var stream = PluginAssembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
				{
					Plugin.Log.LogError($"Missing embedded sprite resource: {resourceName}");
					return null;
				}

				var bytes = new byte[stream.Length];
				var offset = 0;
				while (offset < bytes.Length)
				{
					var read = stream.Read(bytes, offset, bytes.Length - offset);
					if (read <= 0)
					{
						break;
					}
					offset += read;
				}

				var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
				texture.name = spriteName.Replace(".sprite", ".texture");
				if (!ImageConversion.LoadImage(texture, bytes))
				{
					Plugin.Log.LogError($"Failed to load embedded sprite resource: {resourceName}");
					return null;
				}

				texture.filterMode = FilterMode.Bilinear;
				var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
				sprite.name = spriteName;
				return sprite;
			}
		}

		private sealed class RodDefinition
		{
			public RodDefinition(string id, Sprite sprite)
			{
				Id = id;
				Sprite = sprite;
			}

			public string Id { get; }
			public Sprite Sprite { get; }
		}
	}

	[HarmonyPatch(typeof(ItemManager), "OnItemDataAddressablesLoaded")]
	internal static class ItemManagerLoadedPatch
	{
		private static void Postfix(ItemManager __instance)
		{
			RodLockerRuntime.RegisterRodItems(__instance);
			RodLockerRuntime.EnsureRodLockerGrid();
		}
	}

	[HarmonyPatch(typeof(GameManager), nameof(GameManager.BeginGame))]
	internal static class GameManagerBeginGamePatch
	{
		private static void Postfix()
		{
			RodLockerRuntime.ResetCachedDestination();
			RodLockerRuntime.EnsureRuntimeData();
			RodLockerRuntime.EnsureRodLockerGrid();
		}
	}

	[HarmonyPatch(typeof(GameConfigData), nameof(GameConfigData.GetGridConfigForKey))]
	internal static class GameConfigDataGetGridConfigPatch
	{
		private static void Postfix(GridKey key, ref GridConfiguration __result)
		{
			if (RodLockerRuntime.IsRodLockerGrid(key))
			{
				RodLockerRuntime.EnsureRuntimeData();
				var saveData = GameManager.Instance?.SaveData;
				if (saveData != null && saveData.grids.TryGetValue(key, out var grid))
				{
					__result = grid.GridConfiguration;
				}
			}
		}
	}

	[HarmonyPatch(typeof(DockUI), "ShowUIWithDelay")]
	internal static class DockUIShowUIWithDelayPatch
	{
		private static void Postfix(DockUI __instance, Dock dock, ref IEnumerator __result)
		{
			__result = AddRodLockerButtonAfterVanillaUI(__instance, dock, __result);
		}

		private static IEnumerator AddRodLockerButtonAfterVanillaUI(DockUI dockUi, Dock dock, IEnumerator original)
		{
			while (true)
			{
				object current;
				try
				{
					if (!original.MoveNext())
					{
						break;
					}

					current = original.Current;
				}
				catch (Exception ex)
				{
					Plugin.Log.LogError($"Vanilla DockUI.ShowUIWithDelay failed before Rod Locker injection: {ex}");
					yield break;
				}

				yield return current;
			}

			(original as IDisposable)?.Dispose();
			RodLockerRuntime.AddRodLockerButton(dockUi, dock);
		}
	}

	[HarmonyPatch(typeof(DestinationButton), nameof(DestinationButton.Init))]
	internal static class DestinationButtonInitPatch
	{
		private static void Postfix(DestinationButton __instance)
		{
			if (__instance != null && RodLockerRuntime.IsRodLockerDestination(__instance.destination))
			{
				RodLockerRuntime.SetDestinationButtonText(__instance);
				__instance.StartCoroutine(SetTextNextFrame(__instance));
			}
		}

		private static IEnumerator SetTextNextFrame(DestinationButton button)
		{
			yield return null;
			RodLockerRuntime.SetDestinationButtonText(button);
		}
	}

	[HarmonyPatch(typeof(MarketDestinationUI), "ShowMainUI")]
	internal static class MarketDestinationUIShowMainUIPatch
	{
		private static void Prefix(MarketDestinationUI __instance)
		{
			if (RodLockerRuntime.IsRodLockerMarketUI(__instance))
			{
				RodLockerRuntime.EnsureRodLockerGrid();
			}
		}

		private static void Postfix(MarketDestinationUI __instance)
		{
			if (!RodLockerRuntime.IsRodLockerMarketUI(__instance))
			{
				return;
			}

			RodLockerRuntime.SetMarketTitle(__instance);
			__instance.StartCoroutine(SetTitleNextFrame(__instance));
		}

		private static IEnumerator SetTitleNextFrame(MarketDestinationUI ui)
		{
			yield return null;
			RodLockerRuntime.SetMarketTitle(ui);
		}
	}

	[HarmonyPatch(typeof(MarketDestinationUI), "OnMarketTabChanged")]
	internal static class MarketDestinationUIOnMarketTabChangedPatch
	{
		private static void Postfix(MarketDestinationUI __instance)
		{
			RodLockerRuntime.SetMarketTitle(__instance);
		}
	}
}
