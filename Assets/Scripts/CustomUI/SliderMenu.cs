using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderMenu : MonoBehaviour
{
    const int ONDRAGSCALESPEED = 8;
    //
    const float MOVEBY = 100f;
    //
    enum DIRECTION
    {
        None,
        Left,
        Right
    };
    //
    DIRECTION currentDirection;
    DIRECTION oldDirection;
    //
    //bool resetMenuItemsRotation;
    bool isDragging;
    bool isScreenTouched;
    bool directionChanged;
    //
    int prevItemIndex;
    int currentItemIndex;
    int rightItemIndex;
    int middleItemIndex;
    int leftItemIndex;
    int totalMenuItems;
    //
    float swipeDistance;
    float normalizedSwipeDistance;
    float Y;
    float Z;
    //
    Vector2 firstTouchPosition;
    Vector2 lastTouchPosition;
    //
    Vector3 HIGHLIGHTEDSCALE; 
    public Transform Parent;
    public GameObject bidCardPrefab;
    //
    Transform[] selectableMenuItemsArray; 
    /**
	 ******************************************************************************************
	 * YOU CAN CHANGE ALL OF THE BELOW PUBLIC VARIABLES TO CONSTs [Is why they are named all caps]
	 * AFTER YOU ARE DONE TWEAKING AS PER YOUR REQUIREMENTS.
	 ****************************************************************************************** 
	 */
    //
    [Tooltip("Total MenuItems you want to display in the menu.")]
    public int TOTAL_MENU_ITEMS = 10;
    //
    [Tooltip("Distance between two adjacent MenuItems.")]
    public int ADJACENTDISTANCE = 400;
    //
    public Vector3 DEFAULTSCALE = new Vector3(1, 1, 1);
    //
    [Tooltip("Scale factor of current highlighted MenuItems x times normal scale.")]
    public float HIGHLIGHTEDSCALEFACTOR = 3;
    //
    [Tooltip("Minimum finger/mouse swipe distance(Normalized between 0 -> 1) across screen width which is to be registered as a swipe.")]
    public float SWIPETHRESHOLD = 0.1f;
    //
    [Tooltip("Speed of spin of the currently highlighted MenuItems.")]
    public int ROTATIONSPEED = 50;
    // Start is called before the first frame update
    public RectTransform menuRectTransform;
    public GameObject nextBtn, prevBtn;
    public GameObject startButton;
    Config config;
    void Awake()
    {
        config = StaticDataController.Instance.mConfig;
        setupReferences();

        HIGHLIGHTEDSCALE = DEFAULTSCALE * HIGHLIGHTEDSCALEFACTOR;

        Y = Parent.localPosition.y;
        Z = Parent.localPosition.z;

        loadMenuItems();
    }

    void setupReferences()
    {
        foreach (BidCardItem t in Parent.GetComponentsInChildren<BidCardItem>())
            Destroy(t.gameObject);

        SelectedDidItem = null;

        config.bidCards.ForEach(x => {
            GameObject bidObj = Instantiate(bidCardPrefab, Parent);
            BidCardItem bidItem = bidObj.GetComponent<BidCardItem>();
            bidItem.currentIndex = x.mId;
            bidItem.logo.text = x.mName;
            bidItem.entryFee = x.entryFee;

            var bidBtn = bidObj.GetComponent<Button>();
            if (bidBtn) {
                bidBtn.onClick.RemoveAllListeners();
                bidBtn.onClick.AddListener(()=> OnBidSelected(bidItem));
            }
            if (SelectedDidItem == null) SelectedDidItem = bidItem;//Select first by default.
        });         
    }

    //PLAY BUTTON IN CONFIGURATION SCREEN

    BidCardItem SelectedDidItem=null;
    void OnBidSelected(BidCardItem bidCardItem) {

       // Debug.Log("Card Selected " + bidCardItem.gameModeText.text);
        SelectedDidItem = bidCardItem;
        startButton.GetComponent<Animator>().SetTrigger("Highlight");
    }

    public void OnPlayButton() {
        if (SelectedDidItem != null)
            SelectedDidItem.OnClick();
        else
            Debug.LogError("BidCardItem Not Selected" );
    }

    public void OnChallangeButton()
    {
        if (SelectedDidItem != null)
            SelectedDidItem.OnSendChallenge();
        else
            Debug.LogError("BidCardItem Not Selected");
    }
    //


    void loadMenuItems()
    {
        spawnAllMenuItems(); 

        totalMenuItems = selectableMenuItemsArray.Length;

        if (totalMenuItems == 0)
            Debug.LogError("No MenuItems found!");
    }

    

    void spawnAllMenuItems()
    {

        selectableMenuItemsArray = new Transform[TOTAL_MENU_ITEMS];

        for (int i = 0; i < TOTAL_MENU_ITEMS; i++)
        {
            //GameObject obj = Instantiate(Resources.Load("Prefabs/Players/Cube" + (i + 1))) as GameObject;
            Transform MenuItem = Parent.GetChild(i).transform;
            //MenuItems.SetParent(Parent);
            MenuItem.localScale = DEFAULTSCALE;
            MenuItem.localPosition = new Vector3(i * ADJACENTDISTANCE, 0, -150);
            MenuItem.localRotation = Quaternion.identity;
            MenuItem.name = "MenuItem_" + (i + 1);
            selectableMenuItemsArray[i] = MenuItem;
            if (MenuItem.name.Equals("MenuItem_1"))
            {
                setActiveMenuItems(MenuItem);
            }
        }
        updateButtonsState();
    }

    void setActiveMenuItems(Transform trnsfrm)
    {        
        trnsfrm.localScale = HIGHLIGHTEDSCALE;
        
    }

    void updateButtonsState()
    {
        foreach(Transform t in selectableMenuItemsArray)
            t.GetComponent<Button>().interactable = false;

        selectableMenuItemsArray[currentItemIndex].GetComponent<Button>().interactable = true;
        nextBtn.SetActive(currentItemIndex < TOTAL_MENU_ITEMS-1);
        prevBtn.SetActive(currentItemIndex > 0);


        var carComp = selectableMenuItemsArray[currentItemIndex].GetComponent<BidCardItem>();
        if(carComp)
            OnBidSelected(carComp);


    }
     

    void Update()
    {

        if (buttonDirection == DIRECTION.None)
        {
            //On drag begin
            if (Input.GetMouseButton(0))
            {
                if (!menuRectTransform.rect.Contains(menuRectTransform.InverseTransformPoint(Input.mousePosition)))
                    return;
                //resetMenuItemsRotation = false;
                isScreenTouched = true;
                if (!isDragging)
                {
                    firstTouchPosition = Input.mousePosition;
                    lastTouchPosition = firstTouchPosition;
                    isDragging = true;
                    initLeftRightItemIndex();
                }
                else
                {

                    if (directionChanged)
                        updateLeftRightItemIndex();

                    if (Input.mousePosition.x < lastTouchPosition.x)
                    { //Left drag.
                        CurrentDirection = DIRECTION.Left;
                        Parent.localPosition = Vector3.Lerp(Parent.localPosition, new Vector3(Parent.localPosition.x - MOVEBY, Y, Z), Time.deltaTime);
                        leftSwipe();
                    }
                    else if (Input.mousePosition.x > lastTouchPosition.x)
                    { //Right drag.
                        CurrentDirection = DIRECTION.Right;
                        Parent.localPosition = Vector3.Lerp(Parent.localPosition, new Vector3(Parent.localPosition.x + MOVEBY, Y, Z), Time.deltaTime);
                        rightSwipe();
                    }


                    lastTouchPosition = Input.mousePosition;
                }
            }
            else
            {//When not dragging.
                if (isScreenTouched)
                {
                    if (isDragging)
                        isDragging = false;

                    CurrentDirection = DIRECTION.None;
                    //Calculate distance swiped.
                    if (lastTouchPosition != Vector2.zero)
                    {
                        swipeDistance = lastTouchPosition.x - firstTouchPosition.x;
                        normalizedSwipeDistance = swipeDistance / Screen.width;
                        lastTouchPosition = Vector3.zero;
                    }

                    if (normalizedSwipeDistance < 0)
                        leftSwipe();
                    else if (normalizedSwipeDistance > 0)
                        rightSwipe();

                    //snap to nearest MenuItems
                    if (!isMenuItemselectParentSnapped())
                    {
                        int toMove = (int)((Parent.localPosition.x) % ADJACENTDISTANCE);

                        //Large swipe. Move to next item.
                        if (normalizedSwipeDistance < -SWIPETHRESHOLD || normalizedSwipeDistance > SWIPETHRESHOLD)
                        {
                            toMove += (normalizedSwipeDistance > 0) ? -ADJACENTDISTANCE : (normalizedSwipeDistance < 0) ? ADJACENTDISTANCE : 0;

                            if (Mathf.Abs(toMove) > ADJACENTDISTANCE)
                                toMove %= ADJACENTDISTANCE;
                        }

                        Parent.localPosition = Vector3.Lerp(Parent.localPosition, new Vector3(Parent.localPosition.x - toMove, Y, Z), Time.deltaTime * 15);
                        selectableMenuItemsArray[currentItemIndex].localScale = Vector3.Lerp(selectableMenuItemsArray[currentItemIndex].localScale, DEFAULTSCALE, Time.deltaTime * ROTATIONSPEED);

                        if ((toMove >= -1 && toMove <= 1) || (Parent.localPosition.x > 0) || (Parent.localPosition.x < -(totalMenuItems - 1) * ADJACENTDISTANCE))
                        {
                            //Round off lerp values
                            if (toMove >= -1 && toMove <= 1)
                                Parent.localPosition = new Vector3(Mathf.RoundToInt(Parent.localPosition.x / ADJACENTDISTANCE) * ADJACENTDISTANCE, Y, Z);

                            //If already on first MenuItems snap To First Element
                            if (Parent.localPosition.x > 0)
                                Parent.localPosition = new Vector3(0, Y, Z);

                            //If already on last MenuItems snap To LastElement
                            if (Parent.localPosition.x < -(totalMenuItems - 1) * ADJACENTDISTANCE)
                                Parent.localPosition = new Vector3(-(totalMenuItems - 1) * ADJACENTDISTANCE, Y, Z);

                            isScreenTouched = false;
                            currentItemIndex = -Mathf.RoundToInt(Parent.localPosition.x / ADJACENTDISTANCE);
                            updateButtonsState();
                        }
                    }
                }
                else
                {
                    currentItemIndex = -Mathf.RoundToInt(Parent.localPosition.x / ADJACENTDISTANCE);
                    selectableMenuItemsArray[currentItemIndex].localScale = Vector3.Lerp(selectableMenuItemsArray[currentItemIndex].localScale, HIGHLIGHTEDSCALE, Time.deltaTime * ROTATIONSPEED);
                }

            }
             
        }
        else if(selectableMenuItemsArray[currentItemIndex].localScale!= HIGHLIGHTEDSCALE)
        {
            currentItemIndex = -Mathf.RoundToInt(Parent.localPosition.x / ADJACENTDISTANCE);
            selectableMenuItemsArray[currentItemIndex].localScale = Vector3.Lerp(selectableMenuItemsArray[currentItemIndex].localScale, HIGHLIGHTEDSCALE, Time.deltaTime * ROTATIONSPEED);
        }


        if (prevItemIndex != currentItemIndex)
        {
            prevItemIndex = currentItemIndex;
            UpdateUI();
        }
    }

    void initLeftRightItemIndex()
    {
        leftItemIndex = currentItemIndex - 1;
        rightItemIndex = currentItemIndex + 1;
        middleItemIndex = currentItemIndex;
    }

    void updateLeftRightItemIndex()
    {
        if (CurrentDirection == DIRECTION.Left)
            rightItemIndex = leftItemIndex + 1;
        else if (CurrentDirection == DIRECTION.Right)
            leftItemIndex = rightItemIndex - 1;

        middleItemIndex = -1;
    }

    DIRECTION CurrentDirection
    {
        get
        {
            return currentDirection;
        }
        set
        {
            if (currentDirection != value)
            {
                oldDirection = currentDirection;
                currentDirection = value;

                if (oldDirection != DIRECTION.None)
                {
                    directionChanged = true;
                }

            }
            else
            {
                directionChanged = false;
            }
        }
    }

    void leftSwipe()
    {
        //Scale DOWN leftItem, Scale UP rightItem
        if (leftItemIndex >= 0 && leftItemIndex < totalMenuItems)
            selectableMenuItemsArray[leftItemIndex].localScale = Vector3.Lerp(selectableMenuItemsArray[leftItemIndex].localScale, DEFAULTSCALE, Time.deltaTime * ONDRAGSCALESPEED);
        if (rightItemIndex >= 0 && rightItemIndex < totalMenuItems)
            selectableMenuItemsArray[rightItemIndex].localScale = Vector3.Lerp(selectableMenuItemsArray[rightItemIndex].localScale, HIGHLIGHTEDSCALE, Time.deltaTime * ONDRAGSCALESPEED);
        if (middleItemIndex >= 0 && middleItemIndex < totalMenuItems)
            selectableMenuItemsArray[middleItemIndex].localScale = Vector3.Lerp(selectableMenuItemsArray[middleItemIndex].localScale, DEFAULTSCALE, Time.deltaTime * ONDRAGSCALESPEED);
    }

    void rightSwipe()
    {
        //Scale DOWN rightItem, Scale UP leftItem
        if (leftItemIndex >= 0 && leftItemIndex < totalMenuItems)
            selectableMenuItemsArray[leftItemIndex].localScale = Vector3.Lerp(selectableMenuItemsArray[leftItemIndex].localScale, HIGHLIGHTEDSCALE, Time.deltaTime * ONDRAGSCALESPEED);
        if (rightItemIndex >= 0 && rightItemIndex < totalMenuItems)
            selectableMenuItemsArray[rightItemIndex].localScale = Vector3.Lerp(selectableMenuItemsArray[rightItemIndex].localScale, DEFAULTSCALE, Time.deltaTime * ONDRAGSCALESPEED);
        if (middleItemIndex >= 0 && middleItemIndex < totalMenuItems)
            selectableMenuItemsArray[middleItemIndex].localScale = Vector3.Lerp(selectableMenuItemsArray[middleItemIndex].localScale, DEFAULTSCALE, Time.deltaTime * ONDRAGSCALESPEED);
    }


    void resetFixedScales()
    {
   
        if (leftItemIndex >= 0 && leftItemIndex < totalMenuItems)
            selectableMenuItemsArray[leftItemIndex].localScale = DEFAULTSCALE;//Vector3.Lerp(selectableMenuItemsArray[leftItemIndex].localScale, HIGHLIGHTEDSCALE, Time.deltaTime * ONDRAGSCALESPEED);
        if (rightItemIndex >= 0 && rightItemIndex < totalMenuItems)
            selectableMenuItemsArray[rightItemIndex].localScale = DEFAULTSCALE;//Vector3.Lerp(selectableMenuItemsArray[rightItemIndex].localScale, DEFAULTSCALE, Time.deltaTime * ONDRAGSCALESPEED);
        if (middleItemIndex >= 0 && middleItemIndex < totalMenuItems)
            selectableMenuItemsArray[middleItemIndex].localScale = HIGHLIGHTEDSCALE;//Vector3.Lerp(selectableMenuItemsArray[middleItemIndex].localScale, DEFAULTSCALE, Time.deltaTime * ONDRAGSCALESPEED);
    }


    bool isMenuItemselectParentSnapped()
    {
        return isScreenTouched && (Mathf.RoundToInt(Parent.localPosition.x % ADJACENTDISTANCE) == 0);
    }


    private void LateUpdate()
    {
        if (buttonDirection != DIRECTION.None)
        {
            Parent.localPosition = Vector3.Lerp(Parent.localPosition, new Vector3(Parent.localPosition.x - toMoveButton, Y, Z), Time.deltaTime * 15);
            if ((toMoveButton >= -1 && toMoveButton <= 1) || (buttonDirection== DIRECTION.Left &&  Parent.localPosition.x >= (xOld - toMoveButton) )
                || (buttonDirection == DIRECTION.Right && Parent.localPosition.x <= (xOld - toMoveButton))
                )
            {
                //Round off lerp values
                //if (toMoveButton >= -1 && toMoveButton <= 1)
                    Parent.localPosition = new Vector3(Mathf.RoundToInt(Parent.localPosition.x / ADJACENTDISTANCE) * ADJACENTDISTANCE, Y, Z);

                //If already on first MenuItems snap To First Element
                if (Parent.localPosition.x > 0)
                    Parent.localPosition = new Vector3(0, Y, Z);

                //If already on last MenuItems snap To LastElement
                if (Parent.localPosition.x < -(totalMenuItems - 1) * ADJACENTDISTANCE)
                    Parent.localPosition = new Vector3(-(totalMenuItems - 1) * ADJACENTDISTANCE, Y, Z);

                isScreenTouched = false;
                currentItemIndex = -Mathf.RoundToInt(Parent.localPosition.x / ADJACENTDISTANCE);
                initLeftRightItemIndex();

                resetFixedScales();
                buttonDirection = DIRECTION.None;
                selectableMenuItemsArray[currentItemIndex].localScale = Vector3.Lerp(selectableMenuItemsArray[currentItemIndex].localScale, HIGHLIGHTEDSCALE, Time.deltaTime * ROTATIONSPEED);
                updateButtonsState();
            }
        }


    }

    DIRECTION buttonDirection = DIRECTION.None;
    int toMoveButton = 0;
    float xOld = 0;
    public void NextMenuItems()
    {

        if (buttonDirection != DIRECTION.None || currentItemIndex >= TOTAL_MENU_ITEMS-1)
            return;

        toMoveButton = ADJACENTDISTANCE;
         xOld = Parent.localPosition.x ;
         buttonDirection = DIRECTION.Right;
        

        isScreenTouched = true;
    }

    public void PreviousMenuItems()
    {
        if (buttonDirection != DIRECTION.None || currentItemIndex < 1)        
            return;
       
            
        toMoveButton = ADJACENTDISTANCE * -1;
        xOld = Parent.localPosition.x ;
        buttonDirection = DIRECTION.Left;
        isScreenTouched = true;
    }


    public void UpdateUI()
    {
        if(selectableMenuItemsArray.Length>0)
            selectableMenuItemsArray[currentItemIndex].GetComponent<BidCardItem>().UpdateUI();
    }

    void OnEnable()
    { 
        Parent.localPosition = new Vector3(0, Y, Z);
        isScreenTouched = false;
        currentItemIndex = Mathf.Abs(Mathf.RoundToInt(Parent.localPosition.x / ADJACENTDISTANCE));
    }
     
}
