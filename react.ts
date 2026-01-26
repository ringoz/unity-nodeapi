/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

import { createElement, lazy } from 'react';
import Reconciler from 'react-reconciler';
import Constants from 'react-reconciler/constants.js';
import { Node } from '.';
import { BsodBoundary } from './bsod.tsx';
import * as FiberConfig from './reconciler.ts';

const reconciler = Reconciler(FiberConfig);
reconciler.injectIntoDevTools(undefined as any);

export function createRoot(parent: Node) {
  const isStrictMode = process.env.NODE_ENV !== 'production';
  const concurrentUpdatesByDefaultOverride = false;
  const identifierPrefix = '';
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
export type Props<T> = React.PropsWithChildren<Attrs<T>> & React.RefAttributes<T>;

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
export type Action = () => void;
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
  onAwake: Action;
  onStart: Action;
  onUpdate: Action;
  onFixedUpdate: Action;
  onLateUpdate: Action;
  onDestroy: Action;
  onEnable: Action;
  onDisable: Action;
  onBecameInvisible: Action;
  onBecameVisible: Action;
  onMouseDown: Action;
  onMouseDrag: Action;
  onMouseEnter: Action;
  onMouseExit: Action;
  onMouseOver: Action;
  onMouseUp: Action;
  onMouseUpAsButton: Action;
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

//#endregion generated
