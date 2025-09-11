using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.UI;



namespace HK.Core
{
    public partial class Base
    {

        public virtual void Move(Vector3 endValue)
        {
            if (transform is RectTransform)
            {
                ((RectTransform)transform).anchoredPosition = endValue;
                return;
            }

            transform.position = endValue;
        }

        public virtual void MoveX(float endValue)
        {
            if (transform is RectTransform)
            {
                Vector2 anchoredPosition = ((RectTransform)transform).anchoredPosition;
                anchoredPosition.x = endValue;
                ((RectTransform)transform).anchoredPosition = anchoredPosition;
                return;
            }

            Vector3 position = transform.position;
            position.x = endValue;
            transform.position = position;
        }

        public virtual void MoveY(float endValue)
        {
            if (transform is RectTransform)
            {
                Vector2 anchoredPosition = ((RectTransform)transform).anchoredPosition;
                anchoredPosition.y = endValue;
                ((RectTransform)transform).anchoredPosition = anchoredPosition;
                return;
            }

            Vector3 position = transform.position;
            position.y = endValue;
            transform.position = position;
        }

        public virtual void MoveZ(float endValue)
        {
            if (transform is RectTransform)
                throw new System.NotImplementedException("RectTransfrom does not have anchoredPosition.z");

            Vector3 position = transform.position;
            position.z = endValue;
            transform.position = position;
        }

        public virtual void MoveLocal(Vector3 endValue)
        {
            if (transform is RectTransform)
            {
                ((RectTransform)transform).anchoredPosition = endValue;
                return;
            }

            transform.localPosition = endValue;
        }

        public virtual void MoveLocalX(float endValue)
        {
            if (transform is RectTransform)
            {
                Vector2 anchoredPosition = ((RectTransform)transform).anchoredPosition;
                anchoredPosition.x = endValue;
                ((RectTransform)transform).anchoredPosition = anchoredPosition;
                return;
            }

            Vector3 localPosition = transform.localPosition;
            localPosition.x = endValue;
            transform.localPosition = localPosition;
        }

        public virtual void MoveLocalY(float endValue)
        {
            if (transform is RectTransform)
            {
                Vector2 anchoredPosition = ((RectTransform)transform).anchoredPosition;
                anchoredPosition.y = endValue;
                ((RectTransform)transform).anchoredPosition = anchoredPosition;
                return;
            }

            Vector3 localPosition = transform.localPosition;
            localPosition.y = endValue;
            transform.localPosition = localPosition;
        }

        public virtual void MoveLocalZ(float endValue)
        {
            if (transform is RectTransform)
                throw new System.NotImplementedException("RectTransfrom does not have anchoredPosition.z");

            Vector3 localPosition = transform.localPosition;
            localPosition.z = endValue;
            transform.localPosition = localPosition;
        }

        public virtual void Rotate(Quaternion endValue)
        {
            transform.rotation = endValue;
        }

        public virtual void Rotate(Vector3 endValue)
        {
            transform.eulerAngles = endValue;
        }

        public virtual void RotateX(float endValue)
        {
            Vector3 rotation = transform.eulerAngles;
            rotation.x = endValue;
            transform.eulerAngles = rotation;
        }

        public virtual void RotateY(float endValue)
        {
            Vector3 rotation = transform.eulerAngles;
            rotation.y = endValue;
            transform.eulerAngles = rotation;
        }

        public virtual void RotateZ(float endValue)
        {
            Vector3 rotation = transform.eulerAngles;
            rotation.z = endValue;
            transform.eulerAngles = rotation;
        }

        public virtual void RotateLocal(Quaternion endValue)
        {
            transform.localRotation = endValue;
        }

        public virtual void RotateLocal(Vector3 endValue)
        {
            transform.localEulerAngles = endValue;
        }

        public virtual void RotateLocalX(float endValue)
        {
            Vector3 rotation = transform.localEulerAngles;
            rotation.x = endValue;
            transform.localEulerAngles = rotation;
        }

        public virtual void RotateLocalY(float endValue)
        {
            Vector3 rotation = transform.localEulerAngles;
            rotation.y = endValue;
            transform.localEulerAngles = rotation;
        }

        public virtual void RotateLocalZ(float endValue)
        {
            Vector3 rotation = transform.localEulerAngles;
            rotation.z = endValue;
            transform.localEulerAngles = rotation;
        }

        public virtual void Scale(Vector3 endValue)
        {
            transform.localScale = endValue;
        }

        public virtual void Scale(float endValue)
        {
            Scale(new Vector3(endValue, endValue, endValue));
        }

        public virtual void ScaleX(float endValue)
        {
            Vector3 scale = transform.localScale;
            scale.x = endValue;
            transform.localScale = scale;
        }

        public virtual void ScaleY(float endValue)
        {
            Vector3 scale = transform.localScale;
            scale.y = endValue;
            transform.localScale = scale;
        }

        public virtual void ScaleZ(float endValue)
        {
            Vector3 scale = transform.localScale;
            scale.z = endValue;
            transform.localScale = scale;
        }

        public virtual void Fade(float endValue)
        {
            if (GetComponent<Image>())
            {
                Color color = GetComponent<Image>().color;
                color.a = endValue;
                GetComponent<Image>().color = color;
                return;
            }

            if (GetComponent<Text>())
            {
                Color color = GetComponent<Text>().color;
                color.a = endValue;
                GetComponent<Text>().color = color;
                return;
            }

            throw new NotImplementedException();
        }

        public virtual void Color(Color endValue)
        {
            if (GetComponent<Image>())
            {
                GetComponent<Image>().color = endValue;
                return;
            }

            if (GetComponent<Text>())
            {
                GetComponent<Text>().color = endValue;
                return;
            }

            throw new NotImplementedException();
        }



    }
}
