using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Tweet
{
    //public GameObject TweetGameObject;
    public string tweet;
    //public bool IsUsed;
}

[Serializable]
public class TweetList
{
    public Tweet[] tweets;
}
