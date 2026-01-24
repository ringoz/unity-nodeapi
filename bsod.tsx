/**********************************************************************
 Copyright (c) Vladimir Davidovich. All rights reserved.
***********************************************************************/

import { PureComponent, type ReactNode } from 'react';
import { TextElement, VisualElement } from './react';

interface BsodProps {
  error: Error;
}

export function Bsod({ error }: BsodProps) {
  return (
    <VisualElement
      style-flexGrow-value={1}
      style-justifyContent-value="Center"
      style-backgroundColor-value={[0, 0, 0, 1]}
      style-color-value={[1, 1, 1, 1]}
      style-paddingLeft-value-value={16}>
      <TextElement style-flexShrink-value={0} style-fontSize-value-value={60}>:(</TextElement>
      <TextElement style-flexShrink-value={0} style-fontSize-value-value={20}>{error?.message || error?.toString()}</TextElement>
      <TextElement style-flexShrink-value={0} style-whiteSpace-value="Pre" style-color-value={[0.7, 0.7, 0.7, 1]}>{error?.stack}</TextElement>
    </VisualElement>
  );
}

export class BsodBoundary extends PureComponent<{
  children?: ReactNode;
}, {
  hasError: boolean;
  error: Error;
}> {
  static readonly displayName = "BsodBoundary";

  state = {
    hasError: false,
    error: undefined! as Error
  };

  static getDerivedStateFromError(error: Error) {
    const hasError = !error.message.startsWith("Cannot change attachment");
    return { hasError, error };
  }

  render() {
    const state = this.state;
    if (!state.hasError)
      return this.props.children;

    return <Bsod error={state.error}></Bsod>;
  }
}
