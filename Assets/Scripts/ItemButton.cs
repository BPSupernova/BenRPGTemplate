using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public Image buttonImage;
    public Text amountText;
    public int buttonVal;

    public void Press() {
        if (GameMenu.instance.theMenu.activeInHierarchy) {
            if (GameManager.instance.itemsHeld[buttonVal] != "") {
                GameMenu.instance.SelectItem(GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[buttonVal]));
            }
        }

        if (Shop.instance.shopMenu.activeInHierarchy) {
            if (Shop.instance.buyMenu.activeInHierarchy) {
                Shop.instance.SelectBuyItem(GameManager.instance.GetItemDetails(Shop.instance.itemsForSale[buttonVal]));
            }

            if (Shop.instance.sellMenu.activeInHierarchy) {
                Shop.instance.SelectSellItem(GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[buttonVal]));
            }
        }

        if (BattleManager.instance.itemSelectMenu.activeInHierarchy) {
            if (GameManager.instance.itemsHeld[buttonVal] != "") {
                BattleManager.instance.SelectItem(GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[buttonVal]));
            }
        }
    }
}
