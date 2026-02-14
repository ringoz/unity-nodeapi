/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

import { createElement, lazy, Suspense, use, useEffect, useState, type PropsWithChildren, type SuspenseProps } from 'react';
import Reconciler from 'react-reconciler';
import Constants from 'react-reconciler/constants.js';
import { ChangeEvent, Event, KeyboardEvent, Node, PointerEvent, RoutedEvent } from '.';
import { BsodBoundary } from './bsod.tsx';
import * as FiberConfig from './reconciler.ts';

const reconciler = Reconciler(FiberConfig);
reconciler.injectIntoDevTools(undefined as never);

export function createRoot(parent: Node) {
  const isStrictMode = process.env.NODE_ENV !== 'production';
  const concurrentUpdatesByDefaultOverride = false;
  const identifierPrefix = '';
  /* eslint-disable @typescript-eslint/no-explicit-any */
  const onUncaughtError = (reconciler as any).defaultOnUncaughtError;
  const onCaughtError = (reconciler as any).defaultOnCaughtError;
  const onRecoverableError = (reconciler as any).defaultOnRecoverableError;
  const onDefaultTransitionIndicator = () => { };
  const root = reconciler.createContainer(
    parent,
    Constants.ConcurrentRoot,
    null,
    isStrictMode,
    concurrentUpdatesByDefaultOverride,
    identifierPrefix,
    onUncaughtError,
    onCaughtError,
    onRecoverableError,
    onDefaultTransitionIndicator
  );
  return {
    render: (component: React.ReactNode) => new Promise<void>((resolve, reject) => {
      try {
        // eslint-disable-next-line @typescript-eslint/no-unused-expressions
        isStrictMode || (component = createElement(BsodBoundary, null, component));
        reconciler.updateContainer(component, root, null, resolve);
      } catch (e) {
        reject(e);
      }
    }),
    unmount: () => {
      try {
        reconciler.updateContainerSync(null, root, null);
        reconciler.flushSyncWork();
        return Promise.resolve();
      } catch (e) {
        return Promise.reject(e);
      }
    }
  };
}

/* eslint-disable @typescript-eslint/no-unused-vars */

type IfEquals<X, Y, A = X, B = never> =
  (<T>() => T extends X ? 1 : 2) extends
  (<T>() => T extends Y ? 1 : 2) ? A : B;

type WritableKeys<T> = {
  [P in keyof T]-?: IfEquals<
    { [Q in P]: T[P] },
    { -readonly [Q in P]: T[P] },
    P
  >
}[keyof T];

type PickWritable<T> = Pick<T, WritableKeys<T>>;

export type Attrs<T> = Partial<PickWritable<T>>;
export type TNode<T> = Node;
export type Props<T> = React.PropsWithChildren<Attrs<T>> & React.RefAttributes<TNode<T>>;

export function /* @__PURE__ */ intrinsic<T>(type: string) {
  const render: React.FunctionComponent<Props<T>> = (props) => createElement(type, props);
  render.displayName = type;
  return render;
}

export function /* @__PURE__ */ asset<T = GameObject>(path: string) {
  return lazy(async () => {
    const $$$ = await Node.loadAssetAsync(path);
    const render: React.FunctionComponent<Props<T>> = (props) => createElement("", { $$$, ...props });
    render.displayName = path;
    return { default: render };
  });
}

export function useScene(path: string) {
  const [state] = useState(() => Promise.withResolvers());
  useEffect(() => {
    const promise = Node.loadSceneAsync(path);
    promise.then(state.resolve, state.reject);
    return () => {
      promise.then((scene) => Node.unloadSceneAsync(scene));
    };
  }, [path, state]);
  return state.promise;
}

export function Async({ promise, children }: PropsWithChildren<{ promise: Promise<unknown> }>) {
  use(promise);
  return children;
}

export function Scene({ path, children, ...rest }: { path: string } & SuspenseProps) {
  const promise = useScene(path);
  return createElement(Suspense, rest, createElement(Async, { promise }, children));
}

export type Ptr<T> = object;
export type Boolean = boolean;
export type Int16 = number;
export type UInt16 = number;
export type Int32 = number;
export type UInt32 = number;
export type Int64 = number;
export type UInt64 = number;
export type Single = number;
export type Double = number;
export type Char = string;
export type String = string;
export type Object = object;
export type List<T> = T[];
export type IEnumerable<T> = Iterable<T>;
export type Action<A = unknown> = (a: A) => void;
export type PropertyPath = string;
export type Vector2 = [x: number, y: number];
export type Vector2Int = Vector2;
export type Vector3 = [x: number, y: number, z: number];
export type Vector3Int = Vector3;
export type Vector4 = [x: number, y: number, z: number, w: number];
export type Quaternion = Vector4;
export type Matrix4x4 = [number, number, number, number, number, number, number, number, number, number, number, number, number, number, number, number];
export type Color = [r: number, g: number, b: number, a: number];
export type Rect = [x: number, y: number, w: number, h: number];
export type RectInt = Rect;
export type Bounds = [x: number, y: number, z: number, sx: number, sy: number, sz: number];
export type BoundsInt = Bounds;
export type Version = [major: number, minor: number, build: number, revision: number];

export const Capsule = intrinsic<GameObject>("Capsule");
export const Cube = intrinsic<GameObject>("Cube");
export const Cylinder = intrinsic<GameObject>("Cylinder");
export const Plane = intrinsic<GameObject>("Plane");
export const Quad = intrinsic<GameObject>("Quad");
export const Sphere = intrinsic<GameObject>("Sphere");

/* eslint-disable @typescript-eslint/no-empty-object-type */

//#region generated

export type HideFlags = 'None' | 'HideInHierarchy' | 'HideInInspector' | 'DontSaveInEditor' | 'NotEditable' | 'DontSaveInBuild' | 'DontUnloadUnusedAsset' | 'DontSave' | 'HideAndDontSave';
export interface ObjectBase {
  name: String;
  hideFlags: HideFlags[];
}

export interface GameObject extends ObjectBase {
  readonly transform: Ptr<Transform>;
  //readonly transformHandle: TransformHandle;
  layer: Int32;
  readonly activeSelf: Boolean;
  readonly activeInHierarchy: Boolean;
  isStatic: Boolean;
  tag: String;
  //readonly scene: Scene;
  readonly sceneCullingMask: UInt64;
  readonly gameObject: Ptr<GameObject>;
  onMessage: Action<Event>;
  onAwake: Action<Event>;
  onStart: Action<Event>;
  onUpdate: Action<Event>;
  onFixedUpdate: Action<Event>;
  onLateUpdate: Action<Event>;
  onDestroy: Action<Event>;
  onEnable: Action<Event>;
  onDisable: Action<Event>;
  onBecameInvisible: Action<Event>;
  onBecameVisible: Action<Event>;
  onMouseDown: Action<Event>;
  onMouseDrag: Action<Event>;
  onMouseEnter: Action<Event>;
  onMouseExit: Action<Event>;
  onMouseOver: Action<Event>;
  onMouseUp: Action<Event>;
  onMouseUpAsButton: Action<Event>;
}
export const GameObject = intrinsic<GameObject>("GameObject");

export interface Component extends ObjectBase {
  readonly transform: Ptr<Transform>;
  //readonly transformHandle: TransformHandle;
  readonly gameObject: Ptr<GameObject>;
  tag: String;
}
export const Component = intrinsic<Component>("Component");

export interface Transform extends Component {
  position: Vector3;
  localPosition: Vector3;
  eulerAngles: Vector3;
  localEulerAngles: Vector3;
  right: Vector3;
  up: Vector3;
  forward: Vector3;
  rotation: Quaternion;
  localRotation: Quaternion;
  localScale: Vector3;
  parent: Ptr<Transform>;
  readonly worldToLocalMatrix: Matrix4x4;
  readonly localToWorldMatrix: Matrix4x4;
  readonly root: Ptr<Transform>;
  readonly childCount: Int32;
  readonly lossyScale: Vector3;
  hasChanged: Boolean;
  hierarchyCapacity: Int32;
  readonly hierarchyCount: Int32;
}
export const Transform = intrinsic<Transform>("Transform");

export interface Behaviour extends Component {
  enabled: Boolean;
  readonly isActiveAndEnabled: Boolean;
}
export const Behaviour = intrinsic<Behaviour>("Behaviour");

export interface MonoBehaviour extends Behaviour {
  //readonly destroyCancellationToken: CancellationToken;
  useGUILayout: Boolean;
  readonly didStart: Boolean;
  readonly didAwake: Boolean;
  runInEditMode: Boolean;
}
export const MonoBehaviour = intrinsic<MonoBehaviour>("MonoBehaviour");

export type Position = 'Relative' | 'Absolute';
export type WorldSpaceSizeMode = 'Dynamic' | 'Fixed';
export type PivotReferenceSize = 'BoundingBox' | 'Layout';
export type Pivot = 'Center' | 'TopLeft' | 'TopCenter' | 'TopRight' | 'LeftCenter' | 'RightCenter' | 'BottomLeft' | 'BottomCenter' | 'BottomRight';
export interface UIDocument extends MonoBehaviour {
  //panelSettings: PanelSettings;
  readonly parentUI: Ptr<UIDocument>;
  //visualTreeAsset: VisualTreeAsset;
  readonly rootVisualElement: Ptr<VisualElement>;
  position: Position;
  worldSpaceSizeMode: WorldSpaceSizeMode;
  worldSpaceSize: Vector2;
  pivotReferenceSize: PivotReferenceSize;
  pivot: Pivot;
  sortingOrder: Single;
  //readonly runtimePanel: IRuntimePanel;
}
export const UIDocument = intrinsic<UIDocument>("UIDocument");

export type UsageHints = 'None' | 'DynamicTransform' | 'GroupTransform' | 'MaskContainer' | 'DynamicColor' | 'DynamicPostProcessing' | 'LargePixelCoverage';
export type PickingMode = 'Position' | 'Ignore';
export type LanguageDirection = 'Inherit' | 'LTR' | 'RTL';
export interface VisualElement {
  //readonly focusController: FocusController;
  focusable: Boolean;
  tabIndex: Int32;
  delegatesFocus: Boolean;
  readonly canGrabFocus: Boolean;
  viewDataKey: String;
  userData: Object;
  disablePlayModeTint: Boolean;
  usageHints: UsageHints[];
  readonly scaledPixelsPerPoint: Single;
  readonly layout: Rect;
  readonly contentRect: Rect;
  readonly worldBound: Rect;
  readonly localBound: Rect;
  readonly worldTransform: Matrix4x4;
  readonly hasActivePseudoState: Boolean;
  readonly hasInactivePseudoState: Boolean;
  readonly hasHoverPseudoState: Boolean;
  readonly hasCheckedPseudoState: Boolean;
  readonly hasEnabledPseudoState: Boolean;
  readonly hasDisabledPseudoState: Boolean;
  readonly hasFocusPseudoState: Boolean;
  readonly hasRootPseudoState: Boolean;
  pickingMode: PickingMode;
  name: String;
  readonly enabledInHierarchy: Boolean;
  enabledSelf: Boolean;
  languageDirection: LanguageDirection;
  visible: Boolean;
  //generateVisualContent: Action<MeshGenerationContext>;
  dataSource: Object;
  dataSourcePath: PropertyPath;
  //dataSourceType: Type;
  //readonly experimental: IExperimentalFeatures;
  //readonly hierarchy: Hierarchy;
  readonly parent: Ptr<VisualElement>;
  //readonly panel: IPanel;
  readonly contentContainer: Ptr<VisualElement>;
  //readonly visualTreeAssetSource: VisualTreeAsset;
  readonly childCount: Int32;
  //readonly schedule: IVisualElementScheduler;
  //readonly style: IStyle;
  //readonly resolvedStyle: IResolvedStyle;
  //readonly customStyle: ICustomStyle;
  //readonly styleSheets: VisualElementStyleSheetSet;
  tooltip: String;
  onAttachToPanel: Action<RoutedEvent>;
  onDetachFromPanel: Action<RoutedEvent>;
  onBlur: Action<RoutedEvent>;
  onFocus: Action<RoutedEvent>;
  onFocusOut: Action<RoutedEvent>;
  onFocusIn: Action<RoutedEvent>;
  onChangeBoolean: Action<ChangeEvent>;
  onChangeInt32: Action<ChangeEvent>;
  onChangeSingle: Action<ChangeEvent>;
  onChangeString: Action<ChangeEvent>;
  onChangeRect: Action<ChangeEvent>;
  onInput: Action<ChangeEvent>;
  onGeometryChanged: Action<ChangeEvent>;
  onKeyDown: Action<KeyboardEvent>;
  onKeyUp: Action<KeyboardEvent>;
  onClick: Action<PointerEvent>;
  onWheel: Action<PointerEvent>;
  onPointerCapture: Action<PointerEvent>;
  onPointerCaptureOut: Action<PointerEvent>;
  onPointerDown: Action<PointerEvent>;
  onPointerUp: Action<PointerEvent>;
  onPointerMove: Action<PointerEvent>;
  onPointerEnter: Action<PointerEvent>;
  onPointerLeave: Action<PointerEvent>;
  onPointerOver: Action<PointerEvent>;
  onPointerOut: Action<PointerEvent>;
  onPointerCancel: Action<PointerEvent>;
}
export const VisualElement = intrinsic<VisualElement>("VisualElement");

export interface BindableElement extends VisualElement {
  //binding: IBinding;
  bindingPath: String;
}
export const BindableElement = intrinsic<BindableElement>("BindableElement");

export interface TextElement extends BindableElement {
  //PostProcessTextVertices: Action<GlyphsEnumerable>;
  text: String;
  enableRichText: Boolean;
  emojiFallbackSupport: Boolean;
  parseEscapeSequences: Boolean;
  displayTooltipWhenElided: Boolean;
  readonly isElided: Boolean;
  //readonly selection: ITextSelection;
}
export const TextElement = intrinsic<TextElement>("TextElement");

export interface Button extends TextElement {
  //clickable: Clickable;
  //iconImage: Background;
}
export const Button = intrinsic<Button>("Button");

export interface ToggleButtonGroup extends BindableElement {
  //value: ToggleButtonGroupState;
  readonly labelElement: Ptr<Label>;
  label: String;
  showMixedValue: Boolean;
  isMultipleSelection: Boolean;
  allowEmptySelection: Boolean;
}
export const ToggleButtonGroup = intrinsic<ToggleButtonGroup>("ToggleButtonGroup");

export type ScaleMode = 'StretchToFill' | 'ScaleAndCrop' | 'ScaleToFit';
export interface Image extends VisualElement {
  //image: Texture;
  //sprite: Sprite;
  //vectorImage: VectorImage;
  sourceRect: Rect;
  uv: Rect;
  scaleMode: ScaleMode;
  tintColor: Color;
}
export const Image = intrinsic<Image>("Image");

export interface Label extends TextElement {
}
export const Label = intrinsic<Label>("Label");

export interface RepeatButton extends TextElement {
}
export const RepeatButton = intrinsic<RepeatButton>("RepeatButton");

export type ScrollerVisibility = 'Auto' | 'AlwaysVisible' | 'Hidden';
export type TouchScrollBehavior = 'Unrestricted' | 'Elastic' | 'Clamped';
export type NestedInteractionKind = 'Default' | 'StopScrolling' | 'ForwardScrolling';
export type ScrollViewMode = 'Vertical' | 'Horizontal' | 'VerticalAndHorizontal';
export interface ScrollView extends VisualElement {
  horizontalScrollerVisibility: ScrollerVisibility;
  verticalScrollerVisibility: ScrollerVisibility;
  elasticAnimationIntervalMs: Int64;
  scrollOffset: Vector2;
  horizontalPageSize: Single;
  verticalPageSize: Single;
  mouseWheelScrollSize: Single;
  scrollDecelerationRate: Single;
  elasticity: Single;
  touchScrollBehavior: TouchScrollBehavior;
  nestedInteractionKind: NestedInteractionKind;
  readonly contentViewport: Ptr<VisualElement>;
  readonly horizontalScroller: Ptr<Scroller>;
  readonly verticalScroller: Ptr<Scroller>;
  mode: ScrollViewMode;
}
export const ScrollView = intrinsic<ScrollView>("ScrollView");

export type SliderDirection = 'Horizontal' | 'Vertical';
export interface Scroller extends VisualElement {
  readonly slider: Ptr<Slider>;
  readonly lowButton: Ptr<RepeatButton>;
  readonly highButton: Ptr<RepeatButton>;
  value: Single;
  lowValue: Single;
  highValue: Single;
  direction: SliderDirection;
}
export const Scroller = intrinsic<Scroller>("Scroller");

export interface Slider extends BindableElement {
  value: Single;
  readonly labelElement: Ptr<Label>;
  label: String;
  showMixedValue: Boolean;
  lowValue: Single;
  highValue: Single;
  readonly range: Single;
  pageSize: Single;
  showInputField: Boolean;
  fill: Boolean;
  direction: SliderDirection;
  inverted: Boolean;
}
export const Slider = intrinsic<Slider>("Slider");

export interface GroupBox extends BindableElement {
  text: String;
}
export const GroupBox = intrinsic<GroupBox>("GroupBox");

export interface RadioButton extends BindableElement {
  value: Boolean;
  readonly labelElement: Ptr<Label>;
  label: String;
  showMixedValue: Boolean;
  toggleOnLabelClick: Boolean;
  text: String;
}
export const RadioButton = intrinsic<RadioButton>("RadioButton");

export interface RadioButtonGroup extends BindableElement {
  value: Int32;
  readonly labelElement: Ptr<Label>;
  label: String;
  showMixedValue: Boolean;
  choices: IEnumerable<String>;
}
export const RadioButtonGroup = intrinsic<RadioButtonGroup>("RadioButtonGroup");

export interface Toggle extends BindableElement {
  value: Boolean;
  readonly labelElement: Ptr<Label>;
  label: String;
  showMixedValue: Boolean;
  toggleOnLabelClick: Boolean;
  text: String;
}
export const Toggle = intrinsic<Toggle>("Toggle");

export type TouchScreenKeyboardType = 'Default' | 'ASCIICapable' | 'NumbersAndPunctuation' | 'URL' | 'NumberPad' | 'PhonePad' | 'NamePhonePad' | 'EmailAddress' | 'NintendoNetworkAccount' | 'Social' | 'Search' | 'DecimalPad' | 'OneTimeCode';
export interface TextField extends BindableElement {
  value: String;
  readonly labelElement: Ptr<Label>;
  label: String;
  showMixedValue: Boolean;
  //readonly textSelection: ITextSelection;
  //readonly textEdition: ITextEdition;
  isReadOnly: Boolean;
  isPasswordField: Boolean;
  autoCorrection: Boolean;
  hideMobileInput: Boolean;
  keyboardType: TouchScreenKeyboardType;
  //readonly touchScreenKeyboard: TouchScreenKeyboard;
  maxLength: Int32;
  isDelayed: Boolean;
  maskChar: Char;
  cursorIndex: Int32;
  readonly cursorPosition: Vector2;
  selectIndex: Int32;
  selectAllOnFocus: Boolean;
  selectAllOnMouseUp: Boolean;
  doubleClickSelectsWord: Boolean;
  tripleClickSelectsLine: Boolean;
  text: String;
  emojiFallbackSupport: Boolean;
  verticalScrollerVisibility: ScrollerVisibility;
  multiline: Boolean;
}
export const TextField = intrinsic<TextField>("TextField");

export interface Box extends VisualElement {
}
export const Box = intrinsic<Box>("Box");

export interface PopupField<String> extends BindableElement {
  value: String;
  readonly labelElement: Ptr<Label>;
  label: String;
  showMixedValue: Boolean;
  choices: List<String>;
  readonly text: String;
  //formatSelectedValueCallback: Func<String, String>;
  //formatListItemCallback: Func<String, String>;
  index: Int32;
}
export const PopupField = intrinsic<PopupField<String>>("PopupField`1");

export interface DropdownField extends PopupField<String> {
}
export const DropdownField = intrinsic<DropdownField>("DropdownField");

export type HelpBoxMessageType = 'None' | 'Info' | 'Warning' | 'Error';
export interface HelpBox extends VisualElement {
  text: String;
  messageType: HelpBoxMessageType;
}
export const HelpBox = intrinsic<HelpBox>("HelpBox");

export interface PopupWindow extends TextElement {
}
export const PopupWindow = intrinsic<PopupWindow>("PopupWindow");

export interface ProgressBar extends BindableElement {
  title: String;
  lowValue: Single;
  highValue: Single;
  value: Single;
}
export const ProgressBar = intrinsic<ProgressBar>("ProgressBar");

export type SelectionType = 'None' | 'Single' | 'Multiple';
export type AlternatingRowBackground = 'None' | 'ContentOnly' | 'All';
export type CollectionVirtualizationMethod = 'FixedHeight' | 'DynamicHeight';
export type BindingSourceSelectionMode = 'Manual' | 'AutoAssign';
export type ListViewReorderMode = 'Simple' | 'Animated';
export interface ListView extends BindableElement {
  //itemsSource: IList;
  selectionType: SelectionType;
  readonly selectedItem: Object;
  readonly selectedItems: IEnumerable<Object>;
  selectedIndex: Int32;
  readonly selectedIndices: IEnumerable<Int32>;
  readonly selectedIds: IEnumerable<Int32>;
  //readonly viewController: BaseListViewController;
  showBorder: Boolean;
  reorderable: Boolean;
  horizontalScrollingEnabled: Boolean;
  showAlternatingRowBackgrounds: AlternatingRowBackground;
  virtualizationMethod: CollectionVirtualizationMethod;
  fixedItemHeight: Single;
  showBoundCollectionSize: Boolean;
  showFoldoutHeader: Boolean;
  headerTitle: String;
  //makeHeader: Func<VisualElement>;
  //makeFooter: Func<VisualElement>;
  showAddRemoveFooter: Boolean;
  bindingSourceSelectionMode: BindingSourceSelectionMode;
  reorderMode: ListViewReorderMode;
  //makeNoneElement: Func<VisualElement>;
  allowAdd: Boolean;
  //overridingAddButtonBehavior: Action<BaseListView, Button>;
  //onAdd: Action<BaseListView>;
  allowRemove: Boolean;
  //onRemove: Action<BaseListView>;
  //makeItem: Func<VisualElement>;
  //itemTemplate: VisualTreeAsset;
  //bindItem: Action<VisualElement, Int32>;
  //unbindItem: Action<VisualElement, Int32>;
  //destroyItem: Action<VisualElement>;
}
export const ListView = intrinsic<ListView>("ListView");

export type TwoPaneSplitViewOrientation = 'Horizontal' | 'Vertical';
export interface TwoPaneSplitView extends VisualElement {
  readonly fixedPane: Ptr<VisualElement>;
  readonly flexedPane: Ptr<VisualElement>;
  fixedPaneIndex: Int32;
  fixedPaneInitialDimension: Single;
  orientation: TwoPaneSplitViewOrientation;
}
export const TwoPaneSplitView = intrinsic<TwoPaneSplitView>("TwoPaneSplitView");

export interface TreeView extends BindableElement {
  //readonly itemsSource: IList;
  selectionType: SelectionType;
  readonly selectedItem: Object;
  readonly selectedItems: IEnumerable<Object>;
  selectedIndex: Int32;
  readonly selectedIndices: IEnumerable<Int32>;
  readonly selectedIds: IEnumerable<Int32>;
  //readonly viewController: TreeViewController;
  showBorder: Boolean;
  reorderable: Boolean;
  horizontalScrollingEnabled: Boolean;
  showAlternatingRowBackgrounds: AlternatingRowBackground;
  virtualizationMethod: CollectionVirtualizationMethod;
  fixedItemHeight: Single;
  autoExpand: Boolean;
  //makeItem: Func<VisualElement>;
  //itemTemplate: VisualTreeAsset;
  //bindItem: Action<VisualElement, Int32>;
  //unbindItem: Action<VisualElement, Int32>;
  //destroyItem: Action<VisualElement>;
}
export const TreeView = intrinsic<TreeView>("TreeView");

export interface Foldout extends BindableElement {
  toggleOnLabelClick: Boolean;
  text: String;
  value: Boolean;
}
export const Foldout = intrinsic<Foldout>("Foldout");

export type ColumnSortingMode = 'None' | 'Default' | 'Custom';
export interface MultiColumnListView extends BindableElement {
  //itemsSource: IList;
  selectionType: SelectionType;
  readonly selectedItem: Object;
  readonly selectedItems: IEnumerable<Object>;
  selectedIndex: Int32;
  readonly selectedIndices: IEnumerable<Int32>;
  readonly selectedIds: IEnumerable<Int32>;
  //readonly viewController: MultiColumnListViewController;
  showBorder: Boolean;
  reorderable: Boolean;
  horizontalScrollingEnabled: Boolean;
  showAlternatingRowBackgrounds: AlternatingRowBackground;
  virtualizationMethod: CollectionVirtualizationMethod;
  fixedItemHeight: Single;
  showBoundCollectionSize: Boolean;
  showFoldoutHeader: Boolean;
  headerTitle: String;
  //makeHeader: Func<VisualElement>;
  //makeFooter: Func<VisualElement>;
  showAddRemoveFooter: Boolean;
  bindingSourceSelectionMode: BindingSourceSelectionMode;
  reorderMode: ListViewReorderMode;
  //makeNoneElement: Func<VisualElement>;
  allowAdd: Boolean;
  //overridingAddButtonBehavior: Action<BaseListView, Button>;
  //onAdd: Action<BaseListView>;
  allowRemove: Boolean;
  //onRemove: Action<BaseListView>;
  //readonly sortedColumns: IEnumerable<SortColumnDescription>;
  //readonly columns: Columns;
  //readonly sortColumnDescriptions: SortColumnDescriptions;
  sortingMode: ColumnSortingMode;
}
export const MultiColumnListView = intrinsic<MultiColumnListView>("MultiColumnListView");

export interface MultiColumnTreeView extends BindableElement {
  //readonly itemsSource: IList;
  selectionType: SelectionType;
  readonly selectedItem: Object;
  readonly selectedItems: IEnumerable<Object>;
  selectedIndex: Int32;
  readonly selectedIndices: IEnumerable<Int32>;
  readonly selectedIds: IEnumerable<Int32>;
  //readonly viewController: MultiColumnTreeViewController;
  showBorder: Boolean;
  reorderable: Boolean;
  horizontalScrollingEnabled: Boolean;
  showAlternatingRowBackgrounds: AlternatingRowBackground;
  virtualizationMethod: CollectionVirtualizationMethod;
  fixedItemHeight: Single;
  autoExpand: Boolean;
  //readonly sortedColumns: IEnumerable<SortColumnDescription>;
  //readonly columns: Columns;
  //readonly sortColumnDescriptions: SortColumnDescriptions;
  sortingMode: ColumnSortingMode;
}
export const MultiColumnTreeView = intrinsic<MultiColumnTreeView>("MultiColumnTreeView");

export interface Tab extends VisualElement {
  readonly tabHeader: Ptr<VisualElement>;
  label: String;
  //iconImage: Background;
  closeable: Boolean;
}
export const Tab = intrinsic<Tab>("Tab");

export interface TabView extends VisualElement {
  readonly contentViewport: Ptr<VisualElement>;
  activeTab: Ptr<Tab>;
  selectedTabIndex: Int32;
  reorderable: Boolean;
}
export const TabView = intrinsic<TabView>("TabView");

//#endregion generated
