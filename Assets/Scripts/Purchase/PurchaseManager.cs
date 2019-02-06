using UnityEngine;
using System.Collections;
using UnityEngine.Purchasing;

public class PurchaseManager : MonoBehaviour, IStoreListener
{

    private IStoreController controller;

    void Start()
    {

        var module = StandardPurchasingModule.Instance();
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
        builder.AddProduct("com.Minesweeper.Init15", ProductType.NonConsumable);
        builder.AddProduct("com.Minesweeper.Init25", ProductType.NonConsumable);
        UnityPurchasing.Initialize(this, builder);

    }

    /// <summary>
    /// Called when Unity IAP is ready to make purchases.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {

        this.controller = controller;

    }

    /// <summary>
    /// Called when Unity IAP encounters an unrecoverable initialization error.
    ///
    /// Note that this will not be called if Internet is unavailable; Unity IAP
    /// will attempt initialization until it becomes available.
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {



    }

    /// <summary>
    /// Called when a purchase completes.
    ///
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {

        return PurchaseProcessingResult.Complete;

    }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
    {



    }

    public void OnPurchaseClicked(string productId)
    {

        controller.InitiatePurchase(productId);

    }

}