# ugui---multi-display
ugui分屏显示，主要解决UGUI在分屏显示时相互干扰BUG，只适用Windows平台！

项目相关类介绍：
* MultiDisplayGraphicRaycaster  
造成分屏显示相互干扰的原因在于：当鼠标点击屏幕时，所有的GraphicRaycaster都会发出射线检测，从而造成其他屏幕UI被点击。MultiDisplayGraphicRaycaster用来替换内置的GraphicRaycaster。  

* MultiDisplayEventSystem  
继承自EventSystem，解决 **currentSelected** 冲突问题，当场景只存在一个EventSystem时，分屏之间只能共享选择的物体。（例如：当在分屏A打开一个Dropdown时，在分屏B点击UI，会造成分屏A的dropDown收起）

* MultiDisplayDropdown
因为Dropdown组件使用了内置的GraphicRaycaster，所以如果想要正常使用Dropdown组件，就需要将内置GraphicRaycaster替换成MultiDisplayGraphicRaycaster。  

```  
所有涉及GraphicRaycaster的组件，都需要改成MultiDisplayGraphicRaycaster
```