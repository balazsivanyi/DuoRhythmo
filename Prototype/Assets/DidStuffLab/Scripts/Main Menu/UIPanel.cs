using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DidStuffLab.Scripts.Main_Menu
{
    public class UIPanel : MonoBehaviour
    {
        public int panelId = 0;
        public int panelToReturnTo = 0;
        private List<Vector2> _sizes = new List<Vector2>();
        private List<Vector2> _positions = new List<Vector2>();

        private List<Transform> _blurPanels = new List<Transform>();

    
        public List<Vector2> BlurPosition()
        {
            _blurPanels = transform.GetComponentsInChildren<Transform>().Where(r => r.CompareTag("BlurPlaceholder")).ToList();
        
            foreach (var blurPanel in _blurPanels)
            {
                var position = blurPanel.GetComponent<RectTransform>().anchoredPosition;
                _positions.Add(position);
            }
        
            return _positions;
        }

        public List<Vector2> BlurScales()
        {
            foreach (var blurPanel in _blurPanels)
            {
                var size = blurPanel.GetComponent<RectTransform>().sizeDelta;
                _sizes.Add(size);
            
            }

            return _sizes;
        }

        public int NumberOfPanels()
        {
            return _blurPanels.Count;
        }

        public void DeactivatePlaceholder()
        {
            foreach (var blurPanel in _blurPanels) blurPanel.gameObject.SetActive(false);
        }

        public virtual void ExecuteSpecificChanges()
        {
        
        }
    
    }
}
