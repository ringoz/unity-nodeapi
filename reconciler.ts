/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

import type Reconciler from 'react-reconciler';
import Constants from 'react-reconciler/constants.js';
import { Node } from '.';
import { name as packageName, version as packageVersion } from './package.json';

export const rendererPackageName = packageName;
export const rendererVersion = packageVersion;
export const supportsMutation = true;
export const supportsPersistence = false;
export const supportsHydration = false;
export const isPrimaryRenderer = true;

type Type = string;
type Props = Record<string, any>;
type Container = Node;
type TextNode = Node;
type SuspenseNode = Node;
type PublicNode = Node;
type HostContext = any;

const EMPTY = Object.freeze({});

export function createInstance<T extends Node>(type: Type, props: Props, root: Container, hostContext: HostContext, internalHandle: Reconciler.OpaqueHandle): T {
  const { $$$, ...rest } = props;
  const instance = Node.create($$$ ?? type) as T;
  commitUpdate(instance, type, EMPTY, rest, internalHandle);
  return instance;
}

export function createTextInstance(text: string, root: Container, hostContext: HostContext, internalHandle: Reconciler.OpaqueHandle): TextNode {
  throw new Error('Method not implemented.');
}

export const appendInitialChild = appendChild;

export function finalizeInitialChildren(instance: Node, type: Type, props: Props, root: Container, hostContext: HostContext): boolean {
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

export function getPublicInstance(instance: Node | TextNode): PublicNode {
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

export function getInstanceFromScope(scopeInstance: any): Node | null {
  throw new Error('Method not implemented.');
}

export function detachDeletedInstance(node: Node): void {
  node.dispose();
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

export function appendChild(parentInstance: Node, child: Node | TextNode): void {
  child.setParent(parentInstance);
}

export function appendChildToContainer(container: Container, child: Node | TextNode): void {
  appendChild(container, child);
}

export function insertBefore(parentInstance: Node, child: Node | TextNode, beforeChild: Node | TextNode | SuspenseNode): void {
  child.setParent(parentInstance, beforeChild);
}

export function insertInContainerBefore(container: Container, child: Node | TextNode, beforeChild: Node | TextNode | SuspenseNode): void {
  insertBefore(container, child, beforeChild);
}

export function removeChild(parentInstance: Node, child: Node | TextNode | SuspenseNode): void {
  child.setParent(null!);
}

export function removeChildFromContainer(container: Container, child: Node | TextNode | SuspenseNode): void {
  removeChild(container, child);
}

export function resetTextContent(instance: Node): void {
  throw new Error('Method not implemented.');
}

export function commitTextUpdate(textInstance: TextNode, oldText: string, newText: string): void {
  throw new Error('Method not implemented.');
}

export function commitMount(instance: Node, type: Type, props: Props, internalInstanceHandle: Reconciler.OpaqueHandle): void {
  throw new Error('Method not implemented.');
}

function isEqual(a: any, b: any) {
  if (a === b) return true;
  if (a instanceof Node && b instanceof Node && a.equals(b)) return true;
  if (Array.isArray(a) && Array.isArray(b) && a.length === b.length && a.every((v, i) => v === b[i])) return true;
  return false;
}

export function commitUpdate(instance: Node, type: Type, prevProps: Props, nextProps: Props, internalHandle: Reconciler.OpaqueHandle): void {
  const { ref: refOld, children: childrenOld, ...restOld } = prevProps;
  const { ref: refNew, children: childrenNew, ...restNew } = nextProps;

  for (const [key, oldVal] of Object.entries(restOld)) {
    const newVal = nextProps[key];
    if (isEqual(oldVal, newVal))
      delete restNew[key];
    else if (newVal === undefined)
      restNew[key] = undefined;
  }

  instance.setProps(restNew);
}

export function hideInstance(instance: Node | TextNode): void {
  instance.setActive(false);
}

export const hideTextInstance = hideInstance;

export function unhideInstance(instance: Node | TextNode): void {
  instance.setActive(true);
}

export const unhideTextInstance = unhideInstance;

export function clearContainer(container: Container): void {
  container.clear();
}
