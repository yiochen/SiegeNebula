using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class NotificationBoardScript : Singleton<NotificationBoardScript> {

    //public float left = 12.2f;
    //public float bottom = 55.6f;

    //public float lineHeight = 18.5f;
    public Text notification;
    private float alpha;

    private CanvasRenderer notificationRenderer;

    void Start()
    {
        notificationRenderer = notification.GetComponent<CanvasRenderer>();
    }

    public void PushMessage(string message)
    {
        notificationRenderer.SetAlpha(1.0f);
        notification.text = message;
        notification.CrossFadeAlpha(0.0f, 1.0f, true);
    }
}
