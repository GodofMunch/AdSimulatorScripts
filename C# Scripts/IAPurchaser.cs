using UnityEngine;
using UnityEngine.Purchasing;

public class IAPurchaser : MonoBehaviour, IStoreListener
{
    private static IStoreController storeController;          
    private static IExtensionProvider storeExtensionProvider;
    public GameObject shopPanel;

    public static string purchaseId = "100gems";
    void Start()
    {
        if(storeController == null)
            initializePurchasing();
    }

    public void initializePurchasing()
    {
        if (isInitialised())
            return;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct("com.DOSullivan.AdSimulator2020." + purchaseId, ProductType.Consumable);
        
        UnityPurchasing.Initialize(this, builder);
    }

    public void buyConsumable()
    {
        buyProductID(purchaseId);
    }

    public void openStore()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }
    }

    public void closeStore()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    private void buyProductID(string purchaseId)
    {
        if (isInitialised())
        {
            Product product = storeController.products.WithID(purchaseId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                storeController.InitiatePurchase(product);
            }
            else
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            
        }
        else
            Debug.Log("BuyProductID FAIL. Not initialized.");
    }

    private bool isInitialised()
    {
        return storeController != null && storeExtensionProvider != null;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        if (purchaseId.Equals(e.purchasedProduct.definition.id))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", e.purchasedProduct.definition.id));
            Gems gems = FindObjectOfType<Gems>();
            gems.IncreaseGems(100);
        }
        else 
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", e.purchasedProduct.definition.id));
        }
        
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
       Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", 
           i.definition.storeSpecificId, p));
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");
        storeController = controller;
        storeExtensionProvider = extensions;
    }
}