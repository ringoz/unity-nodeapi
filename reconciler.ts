/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

import type Reconciler from 'react-reconciler';
import Constants from 'react-reconciler/constants.js';
import { appendChildObject, clearObject, createObject, deleteObject, hideObject, insertBeforeObject, removeChildObject, setObjectProperty, unhideObject } from '.';
import { name as packageName, version as packageVersion } from './package.json';

export const rendererPackageName = packageName;
export const rendererVersion = packageVersion;
export const supportsMutation = true;
export const supportsPersistence = false;
export const supportsHydration = false;
export const isPrimaryRenderer = true;

type Type = string;
type Props = Record<string, any>;
type Container = object;
type Instance = object;
type TextInstance = Instance;
type SuspenseInstance = Instance;
type PublicInstance = Instance;
type HostContext = any;

const EMPTY = Object.freeze({});

export function createInstance<T extends Instance>(type: Type, props: Props, root: Container, hostContext: HostContext, internalHandle: Reconciler.OpaqueHandle): T {
  const instance = createObject(type) as T;
  commitUpdate(instance, type, EMPTY, props, internalHandle);
  return instance;
}

export function createTextInstance(text: string, root: Container, hostContext: HostContext, internalHandle: Reconciler.OpaqueHandle): TextInstance {
  throw new Error('Method not implemented.');
}

export const appendInitialChild = appendChild;

export function finalizeInitialChildren(instance: Instance, type: Type, props: Props, root: Container, hostContext: HostContext): boolean {
  return false;
}

export function shouldSetTextContent(type: Type, props: Props): boolean {
  return false;
}

export function getRootHostContext(root: Container): HostContext | null {
  return EMPTY;
}

export function getChildHostContext(parentHostContext: HostContext, type: Type, root: Container): HostContext {
  return parentHostContext;
}

export function getPublicInstance(instance: Instance | TextInstance): PublicInstance {
  return instance;
}

export function prepareForCommit(containerInfo: Container): Record<string, any> | null {
  return null;
}

export function resetAfterCommit(containerInfo: Container): void {
}

export function preparePortalMount(containerInfo: Container): void {
  throw new Error('Method not implemented.');
}

export const noTimeout = -1;
export const scheduleTimeout = setTimeout;
export const cancelTimeout = clearTimeout;
export const supportsMicrotasks = true;
export const scheduleMicrotask = queueMicrotask;

let currentUpdatePriority = Constants.NoEventPriority;
export function setCurrentUpdatePriority(newPriority: Reconciler.Lane) {
  currentUpdatePriority = newPriority;
}

export function getCurrentUpdatePriority(): Reconciler.Lane {
  return currentUpdatePriority;
}

export function resolveUpdatePriority(): Reconciler.Lane {
  if (currentUpdatePriority)
    return currentUpdatePriority;
  const event = window?.event;
  switch (event?.type) {
    case 'click':
    case 'contextmenu':
    case 'dblclick':
    case 'pointercancel':
    case 'pointerdown':
    case 'pointerup':
    case 'keydown':
    case 'keyup':
    case 'keypress':
    case 'resize':
      return Constants.DiscreteEventPriority;
    case 'pointermove':
    case 'pointerout':
    case 'pointerover':
    case 'pointerenter':
    case 'pointerleave':
    case 'wheel':
      return Constants.ContinuousEventPriority;
    default:
      return Constants.DefaultEventPriority;
  }
}

let schedulerEvent: Event | undefined = undefined;
export function trackSchedulerEvent() {
  schedulerEvent = window?.event;
}

export function resolveEventType(): null | string {
  const event = window?.event;
  return event && event !== schedulerEvent ? event.type : null;
}

export function resolveEventTimeStamp(): number {
  const event = window?.event;
  return event && event !== schedulerEvent ? event.timeStamp : -1.1;
}

export function requestPostPaintCallback() {
}

export function resetFormInstance() {
}

export const NotPendingTransition = null;
export const HostTransitionContext = null as any;

export function shouldAttemptEagerTransition() {
  return false;
}

export function getInstanceFromNode(node: any): Reconciler.Fiber | null | undefined {
  return null;
}

export function beforeActiveInstanceBlur(): void {
  throw new Error('Method not implemented.');
}

export function afterActiveInstanceBlur(): void {
  throw new Error('Method not implemented.');
}

export function prepareScopeUpdate(scopeInstance: any, instance: any): void {
  throw new Error('Method not implemented.');
}

export function getInstanceFromScope(scopeInstance: any): Instance | null {
  throw new Error('Method not implemented.');
}

export function detachDeletedInstance(node: Instance): void {
  deleteObject(node);
}

export function maySuspendCommit(type: Type, props: Props) {
  return false;
}

export function preloadInstance(type: Type, props: Props) {
  return true; // Return true to indicate it's already loaded
}

export function startSuspendingCommit() {
}

export function suspendInstance(type: Type, props: Props) {
}

export function waitForCommitToBeReady() {
  return null;
}

export function appendChild(parentInstance: Instance, child: Instance | TextInstance): void {
  appendChildObject(parentInstance, child);
}

export function appendChildToContainer(container: Container, child: Instance | TextInstance): void {
  appendChild(container, child);
}

export function insertBefore(parentInstance: Instance, child: Instance | TextInstance, beforeChild: Instance | TextInstance | SuspenseInstance): void {
  insertBeforeObject(parentInstance, child, beforeChild);
}

export function insertInContainerBefore(container: Container, child: Instance | TextInstance, beforeChild: Instance | TextInstance | SuspenseInstance): void {
  insertBefore(container, child, beforeChild);
}

export function removeChild(parentInstance: Instance, child: Instance | TextInstance | SuspenseInstance): void {
  removeChildObject(parentInstance, child);
}

export function removeChildFromContainer(container: Container, child: Instance | TextInstance | SuspenseInstance): void {
  removeChild(container, child);
}

export function resetTextContent(instance: Instance): void {
  throw new Error('Method not implemented.');
}

export function commitTextUpdate(textInstance: TextInstance, oldText: string, newText: string): void {
  throw new Error('Method not implemented.');
}

export function commitMount(instance: Instance, type: Type, props: Props, internalInstanceHandle: Reconciler.OpaqueHandle): void {
  throw new Error('Method not implemented.');
}

function isEqual(a: any, b: any) {
  if (a === b) return true;
  //if (isObject(a) && isObject(b) && a.isEqual(b)) return true;
  if (Array.isArray(a) && Array.isArray(b) && a.every((v, i) => v === b[i])) return true;
  return false;
}

export function commitUpdate(instance: Instance, type: Type, prevProps: Props, nextProps: Props, internalHandle: Reconciler.OpaqueHandle): void {
  const { ref: refOld, children: childrenOld, ...restOld } = prevProps;
  const { ref: refNew, children: childrenNew, ...restNew } = nextProps;

  for (const [oldKey, oldVal] of Object.entries(restOld)) {
    const newVal = nextProps[oldKey];
    if (isEqual(oldVal, newVal))
      continue;

    setObjectProperty(instance, oldKey, newVal);
  }
}

export function hideInstance(instance: Instance | TextInstance): void {
  hideObject(instance);
}

export const hideTextInstance = hideInstance;

export function unhideInstance(instance: Instance | TextInstance): void {
  unhideObject(instance);
}

export const unhideTextInstance = unhideInstance;

export function clearContainer(container: Container): void {
  clearObject(container);
}
