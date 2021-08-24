using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if IAP
using UnityEngine.Purchasing;
#endif

namespace WKC
{
    public class PurchaserDataModel : BaseMgr<PurchaserDataModel>, IDisposable
    {
        public const string NOADS_ITEM_ID = "com.jgmfacebooksdk.wjld.noads";
        public Dictionary<string, StoreItem> ProductsDict;

        private Purchaser _purchase;
        public PurchaserDataModel()
        {
#if IAP
            Purchaser.Instance.InitializedEventHandler += Initialized;
            Purchaser.Instance.ProcessPurchaseFailEventHandler += BuyFailEventHandler;
            Purchaser.Instance.ProcessPurchaseEventHandler += ProcessPurchase;
            Purchaser.Instance.ProcessPurchaseReceiptEventHandler += ProcessPurchaseReceipt;
#endif
        }

        private void BuyFailEventHandler(ushort obj)
        {
            switch (obj)
            {
                //未初始化
                case 0:
                    ///DataModel.Instance.buyOKContents = LanguageModel.Instance.GetString( "Purchase not initialized" );
                    Debug.Log("Purchase not initialized");
                    break;
                //商品未找到或未初始化
                case 1:
                    //DataModel.Instance.buyOKContents = LanguageModel.Instance.GetString( "Not purchasing product, not found or not available" );
                    Debug.Log("Not purchasing product, not found or not available");
                    break;
                default:
                    //DataModel.Instance.buyOKContents = LanguageModel.Instance.GetString( "Not purchasing product, not found or not available" );
                    Debug.Log("Not purchasing product, not found or not available");
                    break;
            }
            //UIManager.Instance.Show( DataModel.UI.UI_BUYOK );
        }

        public void LoadShop()
        {
            ProductsDict = new Dictionary<string, StoreItem>();

            var products = PurchaserConfig.Instance.products;


            for (int i = 0; i < products.Count; i++)
            {
                StoreItem si = new StoreItem(products[i]);
                si.AddStoreItemEventHandler += AddShopItem;

                if (!ProductsDict.ContainsKey(si.BuyID))
                    ProductsDict.Add(si.BuyID, si);
                else
                    ProductsDict[si.BuyID] = si;
            }

            Purchaser.Instance.Initialize();
        }

        public void Dispose()
        {
#if IAP
            Purchaser.Instance.InitializedEventHandler -= Initialized;
            Purchaser.Instance.ProcessPurchaseEventHandler -= ProcessPurchase;
            Purchaser.Instance.ProcessPurchaseFailEventHandler -= BuyFailEventHandler;
            Purchaser.Instance.ProcessPurchaseReceiptEventHandler -= ProcessPurchaseReceipt;
#endif
        }

#if IAP
        public void BuyProductID(string productID)
        {
            Purchaser.Instance.BuyProductID(productID);
        }

        private void Initialized(ProductCollection productCollection)
        {
            Debug.Log("IAP total count ==>" + productCollection.all.Length);

            for (int i = 0; i < productCollection.all.Length; i++)
            {
                var product = productCollection.all[i];

                Debug.Log("IAP product storeSpecificId ==>" + product.definition.storeSpecificId);
                Debug.Log("IAP availableToPurchase ==>" + product.availableToPurchase);
                //MDebug.LogError(product.ToString());
                if (product.definition.storeSpecificId.StartsWith("buy")) continue;

                //包含在ProductsDict中的都是正是商品，打折商品ID不会出现在ProductDict中
                if (ProductsDict.ContainsKey(product.definition.storeSpecificId))
                {
                    //真正后台配置商品
                    ProductsDict[product.definition.storeSpecificId].Name = product.metadata.localizedTitle;
                    ProductsDict[product.definition.storeSpecificId].Price = product.metadata.localizedPriceString;
                    ProductsDict[product.definition.storeSpecificId].Description = product.metadata.localizedDescription;
                    ProductsDict[product.definition.storeSpecificId].ProductType = product.definition.type;

                    //for (int j = 0; j < GlobalData.Instance.userData.commoditieList.Count; j++)
                    //{
                    //    if (GlobalData.Instance.userData.commoditieList[j].ID == product.definition.storeSpecificId)
                    //    {
                    //        //string price = ProductsDict[product.definition.storeSpecificId].Price.Substring(1, ProductsDict[product.definition.storeSpecificId].Price.Length - 1);
                    //        GlobalData.Instance.userData.commoditieList[j].localPrice = ProductsDict[product.definition.storeSpecificId].Price;
                    //        //GlobalData.Instance.userData.commoditieList[j].explain = ProductsDict[product.definition.storeSpecificId].Description;
                    //        break;
                    //    }
                    //}

                    //if (product.definition.storeSpecificId == NOADS_ITEM_ID)
                    //{
                    //    if (ProductsDict[product.definition.storeSpecificId].IsBought)
                    //    {
                    //        GlobalData.Instance.userData.isVip = true;
                    //    }
                    //    else
                    //    {
                    //        GlobalData.Instance.userData.isVip = false;
                    //    }
                    //}
                }
                else
                {
                    var alternativeItem = ProductsDict.Where(e => e.Value.AlternativeBuyID == product.definition.storeSpecificId);
                    //如果该ID被设置为一个打折商品ID
                    if (alternativeItem.Count() > 0)
                    {
                        var target = alternativeItem.First();
                        target.Value.AlternativePrice = product.metadata.localizedPriceString;
                    }
                    else
                    {
                        ProductsDict.Add(product.definition.storeSpecificId, new StoreItem(product.definition.storeSpecificId)
                        {
                            Name = product.metadata.localizedTitle,
                            Price = product.metadata.localizedPriceString,
                            Description = product.metadata.localizedDescription,
                            ProductType = product.definition.type,
                        });
                    }
                }

                Debug.Log("IAP localizedTitle ==>" + product.metadata.localizedTitle);
                Debug.Log("IAP storeSpecificId ==>" + product.definition.storeSpecificId);
            }
        }
#endif

        private void ProcessPurchase(string productID)
        {
            //商品购买成功逻辑
            if (ProductsDict.ContainsKey(productID))
                ProductsDict[productID].IsBought = true;
            else
            {
                var targets = ProductsDict.Where(e => e.Value.AlternativeBuyID == productID);
                if (targets.Count() > 0)
                {
                    var target = targets.First();
                    ProductsDict[productID].IsBought = true;
                }
            }

            AddShopItem(productID);

            //LitFramework.Input.InputControlManager.Instance.IsEnable = true;
        }

#if IAP
        private void ProcessPurchaseReceipt(string currency, string productID, int amount, Receipt receiptClass)
        {
#if UNITY_ANDROID
            PayloadAndroid receiptPayload = JsonUtility.FromJson<PayloadAndroid>(receiptClass.Payload);
#if GA
        GameAnalyticsSDK.GameAnalytics.NewBusinessEventGooglePlay(currency, amount, "my_item_type", productID, "my_cart_type", receiptPayload.json, receiptPayload.signature);
#endif
#endif
#if UNITY_IOS
            string receiptPayload = receiptClass.Payload;
#if GA
            GameAnalyticsSDK.GameAnalytics.NewBusinessEventIOS( currency, amount, "my_item_type", productID, "my_cart_type", receiptPayload );
#endif
#endif
        }
#endif

        public event Action DelUpdateShopStatus;
        public List<int> boughtIndex = new List<int>();

        public void AddShopItem(string productID, bool isConfig = false)//, ProductMetadata meta = null)
        {
            Debug.Log("========>购买了商品 " + productID);
            //StatisticManager.Instance.DOT( "shop_buy_" + Purchaser.Instance.products.IndexOf( productID ) );

            //移除广告商品
            if (productID == NOADS_ITEM_ID)
            {//AdManager.Instance.RemoveAds();
             //GlobalData.Instance.userData.isVip = true;
             //GameManager.SendBuyCommodity(productID);//Convert.ToInt32(productID)
            }
            //TODO 这里添加根据不同商品增加特别处理规则
            else
            {
                StoreItem result = null;

                //商品购买成功逻辑
                if (ProductsDict.ContainsKey(productID))
                    result = ProductsDict[productID];
                else
                {
                    var targets = ProductsDict.Where(e => e.Value.AlternativeBuyID == productID);
                    if (targets.Count() > 0)
                    {
                        var target = targets.First();
                        result = target.Value;
                    }
                }

                if (result != null)
                {
                    ResolveRewards(result);

                    //GameManager.SendBuyCommodity(result.BuyID);
                    //DataModel.Instance.ResolveRewards(result.Rewards);
                    ////打折商品处理
                    //CheckDiscount( result, productID );
                }

                //TODO 这里添加根据不同商品增加特别处理规则
                //if ( productID == GOLD_PACK )
                //{
                //    AdManager.Instance.RemoveAds();
                //    DataModel2.Instance.UseGoldenEye = true;
                //}
            }
            UpdateShopStatus();

            //AudioManager.Instance.PlaySE( DataModel.Sound.Sound_ShopSucc );
            //DataModel.Instance.buyOKContents = DataModel.BUYOKCONTENTS;
            //UIManager.Instance.Show( DataModel.UI.UI_BUYOK );
        }

        private void ResolveRewards(StoreItem result)
        {
            //var rew = result.Rewards.First();
            //List<RewardData> rewardDataList = new List<RewardData>();
            //foreach (var item in result.Rewards)
            //{
            //    RewardData rewardData = new RewardData();
            //    rewardData.rewaredType = Define.RewardType.Item;
            //    rewardData.itemId = item.Key;
            //    rewardData.reward = item.Value;
            //    rewardDataList.Add(rewardData);
            //}

            //GameManager.OnGetReward(rewardDataList);
        }

        //private void CheckDiscount( StoreItem result, string productID )
        //{
        //    if ( result.BuyID != productID && result.AlternativeBuyID == productID )
        //    {
        //        //TODO 这里添加根据不同商品增加特别处理规则
        //        //switch ( productID )
        //        //{
        //        //    case DISCOUNT_ID:
        //        //        //DataModel2.Instance.UseDiscountSilverPack = true;
        //        //        break;
        //        //    default:
        //        //        break;
        //        //}
        //    }
        //}


        public void UpdateShopStatus(bool needScale = true)
        {
            DelUpdateShopStatus?.Invoke();
        }

        /// <summary>
        /// 获取去广告价格
        /// 保持在商品列表中得最后一个
        /// </summary>
        /// <returns></returns>
        public string GetRemoveAdsPrice()
        {
            return ProductsDict[NOADS_ITEM_ID].Price;
        }

        public string GetShopPrice(string id)
        {
            return (ProductsDict[id].Price);
        }

        public string GetShopName(string id)
        {
            return (ProductsDict[id].Name);
        }

        public string GetShopDes(string id)
        {
            return (ProductsDict[id].Description);
        }
    }

    // Placing the Purchaser class in the CompleteProject namespace allows it to interact with ScoreManager
    // Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
    public class Purchaser : BaseMgr<Purchaser>
#if IAP
    , IStoreListener
#endif
    {
#if IAP
        public event Action<ushort> ProcessPurchaseFailEventHandler;
        public event Action<string> ProcessPurchaseEventHandler;
        public event Action<ProductCollection> InitializedEventHandler;
        public event Action<string, string, int, Receipt> ProcessPurchaseReceiptEventHandler;

        private static IStoreController m_StoreController;          // The Unity Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

        public static string kProductIDConsumable = "consumable";
        public static string kProductIDNonConsumable = "nonconsumable";
        public static string kProductIDSubscription = "subscription";

        // Apple App Store-specific product identifier for the subscription product.
       //private static string kProductNameAppleSubscription = "";//"com.unity3d.subscription.new";
                                                                 // Google Play Store-specific product identifier subscription product.
        //private static string kProductNameGooglePlaySubscription = "";//"com.unity3d.subscription.original";

        public void Initialize()
        {
            Debug.Log("UnityIAP : Init ");
            // If we haven't set up the Unity Purchasing reference
            if (m_StoreController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }
        }
        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            Debug.Log("UnityIAP :====> If we have already connected to Purchasing ...");
            if (IsInitialized())
            {
                // ... we are done here.
                Debug.Log("UnityIAP :====> ....we are done here.");
                return;
            }

            var productDict = PurchaserDataModel.Instance.ProductsDict;
            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            if (Application.platform == RuntimePlatform.IPhonePlayer ||
               Application.platform == RuntimePlatform.OSXPlayer)
            {
                foreach (var product in productDict)
                {
                    builder.AddProduct(product.Value.BuyID, ProductType.Consumable, new IDs
                {
                    {product.Value.BuyID, AppleAppStore.Name }
                });

                    //Discount Item
                    if (!string.IsNullOrEmpty(product.Value.AlternativeBuyID))
                    {
                        builder.AddProduct(product.Value.AlternativeBuyID, ProductType.Consumable, new IDs
                {
                    {product.Value.AlternativeBuyID, AppleAppStore.Name }
                });
                    }
                }
                Debug.Log("IAPUnityIAP : IOS Begin Initial!!");
            }
            //Android && PC 
            else
            {
                foreach (var product in productDict)
                {
                    builder.AddProduct(product.Value.BuyID, ProductType.Consumable, new IDs
                {
                    {product.Value.BuyID, GooglePlay.Name }
                });

                    //Discount Item
                    if (!string.IsNullOrEmpty(product.Value.AlternativeBuyID))
                    {
                        builder.AddProduct(product.Value.AlternativeBuyID, ProductType.Consumable, new IDs
                {
                    {product.Value.AlternativeBuyID, GooglePlay.Name }
                });
                    }
                }
                Debug.Log("IAPUnityIAP : Android Begin Initial!!");
            }

            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(this, builder);
        }
        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }
        //public void BuyConsumable()
        //{
        //    // Buy the consumable product using its general identifier. Expect a response either 
        //    // through ProcessPurchase or OnPurchaseFailed asynchronously.
        //    BuyProductID( kProductIDConsumable );
        //}
        //public void BuyNonConsumable()
        //{
        //    // Buy the non-consumable product using its general identifier. Expect a response either 
        //    // through ProcessPurchase or OnPurchaseFailed asynchronously.
        //    BuyProductID( kProductIDNonConsumable );
        //}
        //public void BuySubscription()
        //{
        //    // Buy the subscription product using its the general identifier. Expect a response either 
        //    // through ProcessPurchase or OnPurchaseFailed asynchronously.
        //    // Notice how we use the general product identifier in spite of this ID being mapped to
        //    // custom store-specific identifiers above.
        //    BuyProductID( kProductIDSubscription );
        //}


        public void BuyProductID(string productId)
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = m_StoreController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.storeSpecificId));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    m_StoreController.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    ProcessPurchaseFailEventHandler?.Invoke(1);
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                ProcessPurchaseFailEventHandler?.Invoke(0);
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
                Purchaser.Instance.Initialize();
            }
        }


        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                Purchaser.Instance.Initialize();
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) =>
                {
                 // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                 // no purchases are available to be restored.
                 Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }




        #region 回调
        //  
        // --- IStoreListener
        //
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;

            if (InitializedEventHandler != null) InitializedEventHandler(m_StoreController.products);
        }


        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        //购买不同商品结束后的处理方法 对应定义的商品
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            foreach (var prod in PurchaserDataModel.Instance.ProductsDict)
            {
                // A consumable product has been purchased by this user.
                if (String.Equals(args.purchasedProduct.definition.id, prod.Value.BuyID, StringComparison.Ordinal))
                {
                    Debug.Log(string.Format("ProcessPurchase: Succeed : '{0}'", args.purchasedProduct.definition.id));

                    ProcessPurchaseEventHandler?.Invoke(prod.Value.BuyID);

                    var product = m_StoreController.products.WithID(prod.Value.BuyID);
                    string receipt = product.receipt;
                    string currency = product.metadata.isoCurrencyCode;
                    int amount = decimal.ToInt32(product.metadata.localizedPrice * 100);
                    Receipt receiptClass = JsonUtility.FromJson<Receipt>(receipt);
                    ProcessPurchaseReceiptEventHandler?.Invoke(currency, prod.Value.BuyID, amount, receiptClass);

                    // Return a flag indicating whether this product has completely been received, or if the application needs 
                    // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
                    // saving purchased products to the cloud, and when that save is delayed. 
                    return PurchaseProcessingResult.Complete;
                }
                else if (!string.IsNullOrEmpty(prod.Value.AlternativeBuyID) && String.Equals(args.purchasedProduct.definition.id, prod.Value.AlternativeBuyID, StringComparison.Ordinal))
                {
                    Debug.Log(string.Format("ProcessPurchase: Succeed : '{0}'", args.purchasedProduct.definition.id));

                    ProcessPurchaseEventHandler?.Invoke(prod.Value.AlternativeBuyID);

                    var product = m_StoreController.products.WithID(prod.Value.AlternativeBuyID);
                    string receipt = product.receipt;
                    string currency = product.metadata.isoCurrencyCode;
                    int amount = decimal.ToInt32(product.metadata.localizedPrice * 100);
                    Receipt receiptClass = JsonUtility.FromJson<Receipt>(receipt);
                    ProcessPurchaseReceiptEventHandler?.Invoke(currency, prod.Value.AlternativeBuyID, amount, receiptClass);

                    // Return a flag indicating whether this product has completely been received, or if the application needs 
                    // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
                    // saving purchased products to the cloud, and when that save is delayed. 
                    return PurchaseProcessingResult.Complete;
                }
            }

            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));

            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed. 
            return PurchaseProcessingResult.Pending;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }

        #endregion
#endif
    }

    //..
    public class Receipt
    {
        public string Store;
        public string TransactionID;
        public string Payload;

        public Receipt()
        {
            Store = TransactionID = Payload = "";
        }

        public Receipt(string store, string transactionID, string payload)
        {
            Store = store;
            TransactionID = transactionID;
            Payload = payload;
        }
    }

    public class PayloadAndroid
    {
        public string json;
        public string signature;

        public PayloadAndroid()
        {
            json = signature = "";
        }

        public PayloadAndroid(string _json, string _signature)
        {
            json = _json;
            signature = _signature;
        }
    }
}